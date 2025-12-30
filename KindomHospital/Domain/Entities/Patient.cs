using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; } = null!;

        [Required]
        public DateTime BirthDate { get; set; }

        // Relations
        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Ordonnance> Ordonnances { get; set; } = new List<Ordonnance>();
    }
}