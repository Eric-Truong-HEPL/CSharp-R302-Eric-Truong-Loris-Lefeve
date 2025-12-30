using KindomHospital.Application.DTOs;
using KindomHospital.Application.Mappers;
using KindomHospital.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KindomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PatientsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Patients.ToListAsync();
            var dtos = list.Select(p => new PatientDto { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, BirthDate = p.BirthDate }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            return Ok(new PatientDto { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName, BirthDate = p.BirthDate });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest(new { message = "FirstName et LastName requis." });

            var first = dto.FirstName.Trim();
            var last = dto.LastName.Trim();
            if (first.Length > 30 || last.Length > 30)
                return BadRequest(new { message = "FirstName/LastName: 30 caracteres max." });

            // birthdate plausibility
            if (dto.BirthDate < new DateTime(1900, 1, 1) || dto.BirthDate > DateTime.Today)
                return BadRequest(new { message = "BirthDate invalide." });

            var entity = new KindomHospital.Domain.Entities.Patient { FirstName = first, LastName = last, BirthDate = dto.BirthDate };
            // check logical uniqueness: LastName, FirstName, BirthDate
            var exists = await _db.Patients.AnyAsync(p => p.FirstName == first && p.LastName == last && p.BirthDate == dto.BirthDate);
            if (exists) return BadRequest(new { message = "Un patient avec ces informations existe deja." });

            _db.Patients.Add(entity);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, new PatientDto { Id = entity.Id, FirstName = entity.FirstName, LastName = entity.LastName, BirthDate = entity.BirthDate });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientUpdateDto dto)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) p.FirstName = dto.FirstName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.LastName)) p.LastName = dto.LastName.Trim();
            if (dto.BirthDate.HasValue) p.BirthDate = dto.BirthDate.Value;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            // business rule: prevent deletion if patient has history
            var hasHistory = await _db.Consultations.AnyAsync(c => c.PatientId == id) || await _db.Ordonnances.AnyAsync(o => o.PatientId == id);
            if (hasHistory) return BadRequest(new { message = "Impossible de supprimer un patient ayant un historique." });
            _db.Patients.Remove(p);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/consultations")]
        public async Task<IActionResult> GetConsultations(int id)
        {
            // include related doctor and patient so mapper receives populated navigations
            var list = await _db.Consultations.Where(c => c.PatientId == id).Include(c => c.Doctor).Include(c => c.Patient).ToListAsync();
            var dtos = list.Select(ConsultationMapper.ToConsultationDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}/ordonnances")]
        public async Task<IActionResult> GetOrdonnances(int id)
        {
            var list = await _db.Ordonnances.Where(o => o.PatientId == id).Include(o => o.Lignes).ThenInclude(l => l.Medicament).ToListAsync();
            var dtos = list.Select(OrdonnanceMapper.ToOrdonnanceDto).ToList();
            return Ok(dtos);
        }
    }
}
