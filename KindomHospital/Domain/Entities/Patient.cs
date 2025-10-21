using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    namespace CliniqueApp.Models
    {
        public class Patient
        {
            public int Id { get; set; }

            [Required]
            [MaxLength(30)]
            public string LastName { get; set; }

            [Required]
            [MaxLength(30)]
            public string FirstName { get; set; }

            [Required]
            public DateTime BirthDate { get; set; }

            // Relations : 1 Patient → N Consultations et N Ordonnances
            public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
            public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
        }
    }
}