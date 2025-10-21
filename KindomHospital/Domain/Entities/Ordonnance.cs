using KindomHospital.Domain.Entities.CliniqueApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KindomHospital.Domain.Entities
{
    public class Ordonnance
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Doctor))]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        [Required]
        [ForeignKey(nameof(Patient))]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [ForeignKey(nameof(Consultation))]
        public int? ConsultationId { get; set; }
        public Consultation Consultation { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string Notes { get; set; }

        // Relations : 1 Ordonnance → N Lignes
        public ICollection<OrdonnanceLigne> Lignes { get; set; } = new List<OrdonnanceLigne>();
    }
}
