using KindomHospital.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KindomHospital.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Specialty))]
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        // Relations : N Médecins → 1 Spécialité ; 1 Médecin → N Consultations et N Ordonnances
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    }
}

