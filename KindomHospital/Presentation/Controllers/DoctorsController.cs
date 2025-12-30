using KindomHospital.Application.DTOs;
using KindomHospital.Application.Mappers;
using KindomHospital.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KindomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DoctorsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var doctors = await _db.Doctors.Include(d => d.Specialty).ToListAsync();
            var dtos = doctors.Select(DoctorMapper.ToDoctorDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var d = await _db.Doctors.Include(x => x.Specialty).FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return NotFound();
            return Ok(DoctorMapper.ToDoctorDto(d));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorCreateDto dto)
        {
            // validate specialty exists
            if (!await _db.Specialties.AnyAsync(s => s.Id == dto.SpecialtyId))
                return BadRequest(new { message = "SpecialtyId invalide." });

            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest(new { message = "FirstName et LastName requis." });

            if (dto.FirstName.Trim().Length > 30 || dto.LastName.Trim().Length > 30)
                return BadRequest(new { message = "FirstName/LastName: 30 caracteres max." });

            var entity = DoctorMapper.ToDoctor(dto);
            _db.Doctors.Add(entity);
            await _db.SaveChangesAsync();

            // Reload the created entity including its Specialty so the mapper has the related data
            var created = await _db.Doctors.Include(d => d.Specialty).FirstOrDefaultAsync(d => d.Id == entity.Id);
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, DoctorMapper.ToDoctorDto(created!));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DoctorUpdateDto dto)
        {
            var doc = await _db.Doctors.FindAsync(id);
            if (doc == null) return NotFound();
            if (dto.SpecialtyId.HasValue && !await _db.Specialties.AnyAsync(s => s.Id == dto.SpecialtyId.Value))
                return BadRequest(new { message = "SpecialtyId invalide." });

            DoctorMapper.UpdateDoctor(dto, doc);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/specialty")]
        public async Task<IActionResult> GetSpecialty(int id)
        {
            var doc = await _db.Doctors.Include(d => d.Specialty).FirstOrDefaultAsync(d => d.Id == id);
            if (doc == null) return NotFound();
            var s = doc.Specialty;
            return Ok(new SpecialtyDto { Id = s.Id, Name = s.Name, Category = s.Category, Description = s.Description });
        }

        [HttpPut("{id}/specialty/{specialtyId}")]
        public async Task<IActionResult> ChangeSpecialty(int id, int specialtyId)
        {
            var doc = await _db.Doctors.FindAsync(id);
            if (doc == null) return NotFound();
            if (!await _db.Specialties.AnyAsync(s => s.Id == specialtyId)) return BadRequest(new { message = "SpecialtyId invalide." });
            doc.SpecialtyId = specialtyId;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _db.Doctors.FindAsync(id);
            if (doc == null) return NotFound();
            var hasHistory = await _db.Consultations.AnyAsync(c => c.DoctorId == id) || await _db.Ordonnances.AnyAsync(o => o.DoctorId == id);
            if (hasHistory) return BadRequest(new { message = "Impossible de supprimer un medecin ayant un historique." });
            _db.Doctors.Remove(doc);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/consultations")]
        public async Task<IActionResult> GetConsultations(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var q = _db.Consultations.Where(c => c.DoctorId == id).AsQueryable();
            if (from.HasValue) q = q.Where(c => c.Date >= from.Value);
            if (to.HasValue) q = q.Where(c => c.Date <= to.Value);
            var list = await q.Include(c => c.Doctor).Include(c => c.Patient).ToListAsync();
            var dtos = list.Select(ConsultationMapper.ToConsultationDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}/patients")]
        public async Task<IActionResult> GetPatients(int id)
        {
            var patientIds = await _db.Consultations.Where(c => c.DoctorId == id).Select(c => c.PatientId).Distinct().ToListAsync();
            var patients = await _db.Patients.Where(p => patientIds.Contains(p.Id)).ToListAsync();
            var dtos = patients.Select(p => new PatientShortDto { Id = p.Id, FirstName = p.FirstName, LastName = p.LastName }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}/ordonnances")]
        public async Task<IActionResult> GetOrdonnances(int id, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var q = _db.Ordonnances.Where(o => o.DoctorId == id).AsQueryable();
            if (from.HasValue) q = q.Where(o => o.Date >= from.Value);
            if (to.HasValue) q = q.Where(o => o.Date <= to.Value);
            var list = await q.Include(o => o.Lignes).ThenInclude(l => l.Medicament).ToListAsync();
            var dtos = list.Select(OrdonnanceMapper.ToOrdonnanceDto).ToList();
            return Ok(dtos);
        }
    }
}
