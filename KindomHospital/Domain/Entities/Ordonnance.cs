//pas de foreign key explicite car EF Core devine les relations selon gpt a check si c'est vrai
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    public class Ordonnance
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public int? ConsultationId { get; set; }
        public Consultation? Consultation { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(255)]
        public string? Notes { get; set; }

        public ICollection<OrdonnanceLigne> Lignes { get; set; } = new List<OrdonnanceLigne>();
    }
}