using System;

namespace KindomHospital.Application.DTOs
{
    public record OrdonnanceLigneDto
    {
        public int Id { get; init; }
        public int OrdonnanceId { get; init; }
        public int MedicamentId { get; init; }
        public MedicamentShortDto? Medicament { get; init; }
        public string Dosage { get; init; } = "";
        public string Frequency { get; init; } = "";
        public string Duration { get; init; } = "";
        public int Quantity { get; init; }
        public string? Instructions { get; init; }
    }

    public record OrdonnanceLigneCreateDto
    {
        public int MedicamentId { get; init; }
        public string Dosage { get; init; } = "";
        public string Frequency { get; init; } = "";
        public string Duration { get; init; } = "";
        public int Quantity { get; init; }
        public string? Instructions { get; init; }
    }

    public record OrdonnanceLigneUpdateDto
    {
        public string? Dosage { get; init; }
        public string? Frequency { get; init; }
        public string? Duration { get; init; }
        public int? Quantity { get; init; }
        public string? Instructions { get; init; }
    }
}