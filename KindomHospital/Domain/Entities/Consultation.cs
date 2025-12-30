
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KindomHospital.Domain.Entities
{
    public class Consultation
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Hour { get; set; }

        [MaxLength(100)]
        public string? Reason { get; set; }

        // Relations : 1 Consultation → 0..N Ordonnances
        public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    }
}
