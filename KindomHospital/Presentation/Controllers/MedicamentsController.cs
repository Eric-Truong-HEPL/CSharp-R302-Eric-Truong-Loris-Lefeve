using KindomHospital.Application.DTOs;
using KindomHospital.Application.Mappers;
using KindomHospital.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KindomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MedicamentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Medicaments.ToListAsync();
            var dtos = list.Select(m => new MedicamentDto { Id = m.Id, Name = m.Name, DosageForm = m.DosageForm, Strength = m.Strength, AtcCode = m.AtcCode }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var m = await _db.Medicaments.FindAsync(id);
            if (m == null) return NotFound();
            return Ok(new MedicamentDto { Id = m.Id, Name = m.Name, DosageForm = m.DosageForm, Strength = m.Strength, AtcCode = m.AtcCode });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicamentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.DosageForm) || string.IsNullOrWhiteSpace(dto.Strength))
                return BadRequest(new { message = "Name, DosageForm et Strength sont requis." });

            var name = dto.Name.Trim();
            var dosage = dto.DosageForm.Trim();
            var strength = dto.Strength.Trim();

            if (name.Length > 100 || dosage.Length > 30 || strength.Length > 30)
                return BadRequest(new { message = "Longueurs max depassees." });

            if (await _db.Medicaments.AnyAsync(x => x.Name == name))
                return BadRequest(new { message = "Medicament avec ce nom existe deja." });

            var entity = new KindomHospital.Domain.Entities.Medicament { Name = name, DosageForm = dosage, Strength = strength, AtcCode = dto.AtcCode };
            _db.Medicaments.Add(entity);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, new MedicamentDto { Id = entity.Id, Name = entity.Name, DosageForm = entity.DosageForm, Strength = entity.Strength, AtcCode = entity.AtcCode });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicamentCreateDto dto)
        {
            var m = await _db.Medicaments.FindAsync(id);
            if (m == null) return NotFound();
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.DosageForm) || string.IsNullOrWhiteSpace(dto.Strength))
                return BadRequest(new { message = "Name, DosageForm et Strength sont requis." });

            var name = dto.Name.Trim();
            var dosage = dto.DosageForm.Trim();
            var strength = dto.Strength.Trim();
            if (name.Length > 100 || dosage.Length > 30 || strength.Length > 30)
                return BadRequest(new { message = "Longueurs max depassees." });

            if (await _db.Medicaments.AnyAsync(x => x.Name == name && x.Id != id))
                return BadRequest(new { message = "Medicament avec ce nom existe deja." });

            m.Name = name;
            m.DosageForm = dosage;
            m.Strength = strength;
            m.AtcCode = dto.AtcCode;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _db.Medicaments.FindAsync(id);
            if (m == null) return NotFound();
            var referenced = await _db.OrdonnanceLignes.AnyAsync(l => l.MedicamentId == id);
            if (referenced) return BadRequest(new { message = "Impossible de supprimer un medicament reference par des lignes d'ordonnance." });
            _db.Medicaments.Remove(m);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/ordonnances")]
        public async Task<IActionResult> GetOrdonnances(int id)
        {
            var ords = await _db.OrdonnanceLignes.Where(l => l.MedicamentId == id).Select(l => l.OrdonnanceId).Distinct().ToListAsync();
            var list = await _db.Ordonnances.Where(o => ords.Contains(o.Id)).Include(o => o.Lignes).ThenInclude(l => l.Medicament).ToListAsync();
            var dtos = list.Select(OrdonnanceMapper.ToOrdonnanceDto).ToList();
            return Ok(dtos);
        }
    }
}
