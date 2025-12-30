using System;
using System.ComponentModel.DataAnnotations;

namespace KindomHospital.Application.DTOs
{
    public record DoctorShortDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
    }

    public record PatientShortDto
    {
        public int Id { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
    }

    public record ConsultationDto
    {
        public int Id { get; init; }
        public int DoctorId { get; init; }
        public DoctorShortDto? Doctor { get; init; }
        public int PatientId { get; init; }
        public PatientShortDto? Patient { get; init; }
        public DateTime Date { get; init; }
        public TimeSpan Hour { get; init; }
        public string? Reason { get; init; }
    }

    public record ConsultationCreateDto
    {
        public int DoctorId { get; init; }
        public int PatientId { get; init; }
        public DateTime Date { get; init; }
        public TimeSpan Hour { get; init; }
        [MaxLength(100, ErrorMessage = "Reason: 100 caractères maximum.")]
        public string? Reason { get; init; }
    }

    public record ConsultationUpdateDto
    {
        public DateTime? Date { get; init; }
        public TimeSpan? Hour { get; init; }
        [MaxLength(100, ErrorMessage = "Reason: 100 caractères maximum.")]
        public string? Reason { get; init; }
    }
}