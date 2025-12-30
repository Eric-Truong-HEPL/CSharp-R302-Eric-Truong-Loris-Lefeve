using System;
using System.Collections.Generic;

namespace KindomHospital.Application.DTOs
{
    public record OrdonnanceDto
    {
        public int Id { get; init; }
        public int DoctorId { get; init; }
        public int PatientId { get; init; }
        public int? ConsultationId { get; init; }
        public DateTime Date { get; init; }
        public string? Notes { get; init; }
        public List<OrdonnanceLigneDto> Lignes { get; init; } = new();
    }

    public record OrdonnanceCreateDto
    {
        public int DoctorId { get; init; }
        public int PatientId { get; init; }
        public int? ConsultationId { get; init; }
        public DateTime Date { get; init; }
        public string? Notes { get; init; }
        public List<OrdonnanceLigneCreateDto> Lignes { get; init; } = new();
    }

    public record OrdonnanceUpdateDto
    {
        public int? ConsultationId { get; init; }
        public DateTime? Date { get; init; }
        public string? Notes { get; init; }
    }
}