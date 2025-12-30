using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    public class Specialty
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = null!;
        //je les ai mit car dans les annexes ils sont présents
        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        // Relations : 1 Spécialité → N Médecins
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
