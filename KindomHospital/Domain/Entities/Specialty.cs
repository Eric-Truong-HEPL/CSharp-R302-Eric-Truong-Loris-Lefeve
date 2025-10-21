using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace KindomHospital.Domain.Entities
{

    public class Specialty
    {
        public int Id { get; set; }

        //il manque le unique mais je sas pas comment faire
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        // Relations : 1 Spécialité → N Médecins
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
