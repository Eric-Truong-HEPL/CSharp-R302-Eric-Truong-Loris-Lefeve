using KindomHospital.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KindomHospital.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; } 

        [Required]
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; } = null!;

        // Relations
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    }
}

