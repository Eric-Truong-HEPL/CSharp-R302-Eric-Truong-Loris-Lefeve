using KindomHospital.Application.DTOs;
using KindomHospital.Application.Mappers;
using KindomHospital.Domain.Entities;
using KindomHospital.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KindomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ConsultationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? doctorId, [FromQuery] int? patientId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            // If any filter param is present, require at least one of doctorId or patientId per spec.
            var hasAnyFilter = doctorId.HasValue || patientId.HasValue || from.HasValue || to.HasValue;
            if (hasAnyFilter && !doctorId.HasValue && !patientId.HasValue)
                return BadRequest(new { message = "Au moins un doctorId ou patientId doit etre fourni." });

            var q = _db.Consultations.AsQueryable();
            if (doctorId.HasValue) q = q.Where(c => c.DoctorId == doctorId.Value);
            if (patientId.HasValue) q = q.Where(c => c.PatientId == patientId.Value);
            if (from.HasValue) q = q.Where(c => c.Date >= from.Value);
            if (to.HasValue) q = q.Where(c => c.Date <= to.Value);
            var list = await q.Include(c => c.Doctor).Include(c => c.Patient).ToListAsync();
            var dtos = list.Select(ConsultationMapper.ToConsultationDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _db.Consultations.Include(c => c.Doctor).Include(c => c.Patient).FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();
            return Ok(ConsultationMapper.ToConsultationDto(c));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConsultationCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // validate doctor/patient
            if (!await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId)) return BadRequest(new { message = "DoctorId invalide." });
            if (!await _db.Patients.AnyAsync(p => p.Id == dto.PatientId)) return BadRequest(new { message = "PatientId invalide." });

            // schedule conflict check
            var exists = await _db.Consultations.AnyAsync(c => c.DoctorId == dto.DoctorId && c.Date == dto.Date && c.Hour == dto.Hour);
            if (exists) return BadRequest(new { message = "Double-booking pour ce medecin a cette date/heure." });

            var entity = ConsultationMapper.ToConsultation(dto);
            _db.Consultations.Add(entity);
            await _db.SaveChangesAsync();
            // reload with navigation properties to avoid null reference in mapper-generated code
            var created = await _db.Consultations.Include(c => c.Doctor).Include(c => c.Patient).FirstOrDefaultAsync(c => c.Id == entity.Id);
            var resultDto = created == null ? ConsultationMapper.ToConsultationDto(entity) : ConsultationMapper.ToConsultationDto(created);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ConsultationUpdateDto dto)
        {

            var c = await _db.Consultations.FindAsync(id);
            if (c == null) return NotFound();

            ConsultationMapper.UpdateConsultation(dto, c);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Consultations.FindAsync(id);
            if (c == null) return NotFound();
            // business: prevent deletion if linked to ordonnances (consultationId optional so handled)
            var linked = await _db.Ordonnances.AnyAsync(o => o.ConsultationId == id);
            if (linked) return BadRequest(new { message = "Impossible de supprimer une consultation liee a des ordonnances." });
            _db.Consultations.Remove(c);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/ordonnances")]
        public async Task<IActionResult> GetOrdonnances(int id)
        {
            var list = await _db.Ordonnances.Where(o => o.ConsultationId == id).Include(o => o.Lignes).ThenInclude(l => l.Medicament).ToListAsync();
            var dtos = list.Select(OrdonnanceMapper.ToOrdonnanceDto).ToList();
            return Ok(dtos);
        }

        [HttpPost("{id}/ordonnances")]
        public async Task<IActionResult> CreateOrdonnanceForConsultation(int id, [FromBody] OrdonnanceCreateDto dto)
        {
            // validate consultation exists
            var cons = await _db.Consultations.FindAsync(id);
            if (cons == null) return NotFound();
            // ensure dto links to same patient/doctor
            if (dto.PatientId != cons.PatientId || dto.DoctorId != cons.DoctorId)
                return BadRequest(new { message = "Incoherence entre consultation et ordonnance (patient/medecin)." });

            var ordDto = dto with { ConsultationId = id };
            // reuse ordonnances create logic minimal: validate lines/meds
            if (ordDto.Lignes == null || ordDto.Lignes.Count == 0) return BadRequest(new { message = "Une ordonnance doit contenir au moins une ligne." });
            foreach (var l in ordDto.Lignes)
            {
                if (l.Quantity <= 0) return BadRequest(new { message = "La quantite doit etre > 0." });
                if (!await _db.Medicaments.AnyAsync(m => m.Id == l.MedicamentId)) return BadRequest(new { message = $"MedicamentId invalide: {l.MedicamentId}" });
            }

            var entity = OrdonnanceMapper.ToOrdonnance(ordDto);
            entity.Lignes = OrdonnanceLigneMapper.ToOrdonnanceLigneList(ordDto.Lignes);
            _db.Ordonnances.Add(entity);
            await _db.SaveChangesAsync();
            // reload with navigation properties to avoid null reference in mapper-generated code
            var created = await _db.Ordonnances
                .Include(o => o.Lignes)
                .ThenInclude(l => l.Medicament)
                .FirstOrDefaultAsync(o => o.Id == entity.Id);
            var result = created == null ? OrdonnanceMapper.ToOrdonnanceDto(entity) : OrdonnanceMapper.ToOrdonnanceDto(created);
            return CreatedAtAction("Get", "Ordonnances", new { id = entity.Id }, result);
        }
    }
}
