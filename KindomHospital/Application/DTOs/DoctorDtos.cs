
using System;

namespace KindomHospital.Application.DTOs
{
    public record DoctorDto
    {
        public int Id { get; init; }
        public int SpecialtyId { get; init; }
        public SpecialtyShortDto? Specialty { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
    }

    public record DoctorCreateDto
    {
        public int SpecialtyId { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
    }

    public record DoctorUpdateDto
    {
        public int? SpecialtyId { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}