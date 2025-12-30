using KindomHospital.Application.DTOs;
using KindomHospital.Application.Mappers;
using KindomHospital.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KindomHospital.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SpecialtiesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var specs = await _db.Specialties.ToListAsync();
            var dtos = specs.Select(s => new SpecialtyDto { Id = s.Id, Name = s.Name, Category = s.Category, Description = s.Description }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var s = await _db.Specialties.FindAsync(id);
            if (s == null) return NotFound();
            var dto = new SpecialtyDto { Id = s.Id, Name = s.Name, Category = s.Category, Description = s.Description };
            return Ok(dto);
        }

        [HttpGet("{id}/doctors")]
        public async Task<IActionResult> GetDoctors(int id)
        {
            // include specialty navigation so nested DTO can be mapped
            var doctors = await _db.Doctors.Where(d => d.SpecialtyId == id).Include(d => d.Specialty).ToListAsync();
            // use mapper to populate nested SpecialtyShortDto
            var dtos = doctors.Select(DoctorMapper.ToDoctorDto).ToList();
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpecialtyDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Name requis." });

            var name = dto.Name.Trim();
            if (name.Length == 0) return BadRequest(new { message = "Name ne peut pas etre vide ou espaces uniquement." });
            if (name.Length > 30) return BadRequest(new { message = "Name: 30 caracteres max." });

            if (await _db.Specialties.AnyAsync(s => s.Name == name))
                return BadRequest(new { message = "Une specialite avec ce nom existe deja." });

            var entity = new KindomHospital.Domain.Entities.Specialty { Name = name, Category = dto.Category, Description = dto.Description };
            _db.Specialties.Add(entity);
            await _db.SaveChangesAsync();
            var result = new SpecialtyDto { Id = entity.Id, Name = entity.Name, Category = entity.Category, Description = entity.Description };
            return CreatedAtAction(nameof(Get), new { id = entity.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpecialtyDto dto)
        {
            var s = await _db.Specialties.FindAsync(id);
            if (s == null) return NotFound();
            if (dto == null) return BadRequest();

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                var name = dto.Name.Trim();
                if (name.Length == 0) return BadRequest(new { message = "Name ne peut pas etre vide ou espaces uniquement." });
                if (name.Length > 30) return BadRequest(new { message = "Name: 30 caracteres max." });
                if (await _db.Specialties.AnyAsync(x => x.Name == name && x.Id != id))
                    return BadRequest(new { message = "Une specialite avec ce nom existe deja." });
                s.Name = name;
            }
            s.Category = dto.Category;
            s.Description = dto.Description;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var s = await _db.Specialties.FindAsync(id);
            if (s == null) return NotFound();
            var hasDoctors = await _db.Doctors.AnyAsync(d => d.SpecialtyId == id);
            if (hasDoctors) return BadRequest(new { message = "Impossible de supprimer une specialite avec des medecins rattaches." });
            _db.Specialties.Remove(s);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
