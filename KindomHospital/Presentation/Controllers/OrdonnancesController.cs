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
    public class OrdonnancesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public OrdonnancesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? doctorId, [FromQuery] int? patientId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            // If any filter param is present, require at least one of doctorId or patientId per spec.
            var hasAnyFilter = doctorId.HasValue || patientId.HasValue || from.HasValue || to.HasValue;
            if (hasAnyFilter && !doctorId.HasValue && !patientId.HasValue)
                return BadRequest(new { message = "Au moins doctorId ou patientId doit etre fourni." });

            var q = _db.Ordonnances.AsQueryable();
            if (doctorId.HasValue) q = q.Where(o => o.DoctorId == doctorId.Value);
            if (patientId.HasValue) q = q.Where(o => o.PatientId == patientId.Value);
            if (from.HasValue) q = q.Where(o => o.Date >= from.Value);
            if (to.HasValue) q = q.Where(o => o.Date <= to.Value);

            var ords = await q.Include(o => o.Lignes).ThenInclude(l => l.Medicament).ToListAsync();
            var dtos = ords.Select(OrdonnanceMapper.ToOrdonnanceDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ord = await _db.Ordonnances
                .Include(o => o.Lignes)
                .ThenInclude(l => l.Medicament)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ord == null) return NotFound();
            return Ok(OrdonnanceMapper.ToOrdonnanceDto(ord));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdonnanceCreateDto dto)
        {
            // basic validation
            if (dto.Lignes == null || dto.Lignes.Count == 0)
                return BadRequest(new { message = "Une ordonnance doit contenir au moins une ligne." });

            // validate existence of doctor, patient, consultation (if provided)
            var doctorExists = await _db.Doctors.AnyAsync(d => d.Id == dto.DoctorId);
            if (!doctorExists) return BadRequest(new { message = "DoctorId invalide." });

            var patientExists = await _db.Patients.AnyAsync(p => p.Id == dto.PatientId);
            if (!patientExists) return BadRequest(new { message = "PatientId invalide." });

            if (dto.ConsultationId.HasValue)
            {
                var cons = await _db.Consultations.FindAsync(dto.ConsultationId.Value);
                if (cons == null) return BadRequest(new { message = "ConsultationId invalide." });
                if (cons.PatientId != dto.PatientId || cons.DoctorId != dto.DoctorId)
                    return BadRequest(new { message = "Incoherence entre l'ordonnance et la consultation (patient/medecin)." });
            }

            // validate medicaments and quantities
            foreach (var ligne in dto.Lignes)
            {
                if (ligne.Quantity <= 0) return BadRequest(new { message = "La quantite doit etre > 0." });
                var medExists = await _db.Medicaments.AnyAsync(m => m.Id == ligne.MedicamentId);
                if (!medExists) return BadRequest(new { message = $"MedicamentId invalide: {ligne.MedicamentId}" });
            }

            var entity = OrdonnanceMapper.ToOrdonnance(dto);
            // map lignes (medicament navigation not set yet)
            entity.Lignes = OrdonnanceLigneMapper.ToOrdonnanceLigneList(dto.Lignes);

            _db.Ordonnances.Add(entity);
            await _db.SaveChangesAsync();

            // reload with navigation properties to avoid null reference in mapper-generated code
            var created = await _db.Ordonnances
                .Include(o => o.Lignes)
                .ThenInclude(l => l.Medicament)
                .FirstOrDefaultAsync(o => o.Id == entity.Id);
            var resultDto = created == null ? OrdonnanceMapper.ToOrdonnanceDto(entity) : OrdonnanceMapper.ToOrdonnanceDto(created);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdonnanceUpdateDto dto)
        {
            var ord = await _db.Ordonnances.FindAsync(id);
            if (ord == null) return NotFound();

            OrdonnanceMapper.UpdateOrdonnance(dto, ord);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ord = await _db.Ordonnances.FindAsync(id);
            if (ord == null) return NotFound();
            _db.Ordonnances.Remove(ord);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // attach/detach consultation
        [HttpPut("{id}/consultation/{consultationId}")]
        public async Task<IActionResult> AttachConsultation(int id, int consultationId)
        {
            var ord = await _db.Ordonnances.FindAsync(id);
            if (ord == null) return NotFound();
            var cons = await _db.Consultations.FindAsync(consultationId);
            if (cons == null) return BadRequest(new { message = "ConsultationId invalide." });
            // ensure coherence: same patient and doctor
            if (cons.PatientId != ord.PatientId || cons.DoctorId != ord.DoctorId)
                return BadRequest(new { message = "Incoherence entre l'ordonnance et la consultation (patient/medecin)." });

            ord.ConsultationId = consultationId;
            await _db.SaveChangesAsync();

            // reload with navigation properties and return the updated resource so client sees the change
            var updated = await _db.Ordonnances
                .Include(o => o.Lignes)
                .ThenInclude(l => l.Medicament)
                .FirstOrDefaultAsync(o => o.Id == ord.Id);
            if (updated == null) return NoContent();
            return Ok(OrdonnanceMapper.ToOrdonnanceDto(updated));
        }

        [HttpDelete("{id}/consultation")]
        public async Task<IActionResult> DetachConsultation(int id)
        {
            var ord = await _db.Ordonnances.FindAsync(id);
            if (ord == null) return NotFound();
            ord.ConsultationId = null;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Lignes endpoints
        [HttpGet("{id}/lignes")]
        public async Task<IActionResult> GetLignes(int id)
        {
            var ord = await _db.Ordonnances
                .Include(o => o.Lignes)
                .ThenInclude(l => l.Medicament)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (ord == null) return NotFound();
            var lignes = ord.Lignes.Select(OrdonnanceLigneMapper.ToOrdonnanceLigneDto).ToList();
            return Ok(lignes);
        }

        [HttpPost("{id}/lignes")]
        public async Task<IActionResult> AddLignes(int id, [FromBody] List<OrdonnanceLigneCreateDto> lignesDto)
        {
            var ord = await _db.Ordonnances
                .Include(o => o.Lignes)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (ord == null) return NotFound();

            if (lignesDto == null || lignesDto.Count == 0)
                return BadRequest(new { message = "Au moins une ligne doit ?tre fournie." });

            foreach (var l in lignesDto)
            {
                if (l.Quantity <= 0) return BadRequest(new { message = "La quantit? doit ?tre > 0." });
                var med = await _db.Medicaments.FindAsync(l.MedicamentId);
                if (med == null) return BadRequest(new { message = $"MedicamentId invalide: {l.MedicamentId}" });

                var entity = OrdonnanceLigneMapper.ToOrdonnanceLigne(l);
                entity.OrdonnanceId = ord.Id;
                ord.Lignes.Add(entity);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/lignes/{ligneId}")]
        public async Task<IActionResult> GetLigne(int id, int ligneId)
        {
            var ligne = await _db.OrdonnanceLignes
                .Include(l => l.Medicament)
                .FirstOrDefaultAsync(l => l.Id == ligneId && l.OrdonnanceId == id);
            if (ligne == null) return NotFound();
            return Ok(OrdonnanceLigneMapper.ToOrdonnanceLigneDto(ligne));
        }

        [HttpPut("{id}/lignes/{ligneId}")]
        public async Task<IActionResult> UpdateLigne(int id, int ligneId, [FromBody] OrdonnanceLigneUpdateDto dto)
        {
            var ligne = await _db.OrdonnanceLignes.FirstOrDefaultAsync(l => l.Id == ligneId && l.OrdonnanceId == id);
            if (ligne == null) return NotFound();

            if (dto.Quantity.HasValue && dto.Quantity.Value <= 0)
                return BadRequest(new { message = "La quantit? doit ?tre > 0." });

            OrdonnanceLigneMapper.UpdateOrdonnanceLigne(dto, ligne);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/lignes/{ligneId}")]
        public async Task<IActionResult> DeleteLigne(int id, int ligneId)
        {
            var ligne = await _db.OrdonnanceLignes.FirstOrDefaultAsync(l => l.Id == ligneId && l.OrdonnanceId == id);
            if (ligne == null) return NotFound();
            _db.OrdonnanceLignes.Remove(ligne);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
