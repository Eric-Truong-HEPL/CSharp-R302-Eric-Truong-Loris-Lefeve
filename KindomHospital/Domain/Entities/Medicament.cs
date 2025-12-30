using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    public class Medicament
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string DosageForm { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Strength { get; set; } = null!;

        [MaxLength(20)]
        public string? AtcCode { get; set; }

        // Relations
        public ICollection<OrdonnanceLigne> OrdonnanceLignes { get; set; } = new List<OrdonnanceLigne>();
    }
}
