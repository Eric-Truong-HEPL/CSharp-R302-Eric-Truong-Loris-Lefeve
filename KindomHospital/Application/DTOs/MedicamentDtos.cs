using System;

namespace KindomHospital.Application.DTOs
{
    public record MedicamentShortDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
    }

    public record MedicamentDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public string DosageForm { get; init; } = "";
        public string Strength { get; init; } = "";
        public string? AtcCode { get; init; }
    }

    public record MedicamentCreateDto
    {
        public string Name { get; init; } = "";
        public string DosageForm { get; init; } = "";
        public string Strength { get; init; } = "";
        public string? AtcCode { get; init; }
    }

    public record MedicamentUpdateDto
    {
        public string? Name { get; init; }
        public string? DosageForm { get; init; }
        public string? Strength { get; init; }
        public string? AtcCode { get; init; }
    }
}