using System;

namespace KindomHospital.Application.DTOs
{
    public record SpecialtyShortDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
    }

    public record SpecialtyDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public string? Category { get; init; }
        public string? Description { get; init; }
    }

    public record SpecialtyCreateDto
    {
        public string Name { get; init; } = "";
        public string? Category { get; init; }
        public string? Description { get; init; }
    }

    public record SpecialtyUpdateDto
    {
        public string? Name { get; init; }
        public string? Category { get; init; }
        public string? Description { get; init; }
    }
}