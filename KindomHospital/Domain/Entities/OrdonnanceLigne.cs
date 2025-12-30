using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KindomHospital.Domain.Entities
{
    public class OrdonnanceLigne
    {
        public int Id { get; set; }

        [Required]
        public int OrdonnanceId { get; set; }
        public Ordonnance Ordonnance { get; set; } = null!;

        [Required]
        public int MedicamentId { get; set; }
        public Medicament Medicament { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Dosage { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Frequency { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Duration { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(255)]
        public string? Instructions { get; set; }
    }
}
