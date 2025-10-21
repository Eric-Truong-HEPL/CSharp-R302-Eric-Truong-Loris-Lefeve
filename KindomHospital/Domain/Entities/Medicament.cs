using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Domain.Entities
{
    public class Medicament
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string DosageForm { get; set; }

        [Required]
        [MaxLength(30)]
        public string Strength { get; set; }

        [MaxLength(20)]
        public string AtcCode { get; set; }

        // Relations : 1 Médicament → N Lignes d’ordonnance
        public ICollection<OrdonnanceLigne> OrdonnanceLignes { get; set; } = new List<OrdonnanceLigne>();
    }
}
