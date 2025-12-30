using System;

namespace KindomHospital.Application.DTOs
{
    public record PatientDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public DateTime BirthDate { get; init; }
    }

    public record PatientCreateDto
    {
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public DateTime BirthDate { get; init; }
    }

    public record PatientUpdateDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? BirthDate { get; init; }
    }
}