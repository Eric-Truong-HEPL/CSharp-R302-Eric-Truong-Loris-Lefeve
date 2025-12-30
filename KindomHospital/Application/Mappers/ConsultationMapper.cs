using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class ConsultationMapper
    {
        public static partial ConsultationDto ToConsultationDto(Consultation entity);
        public static partial Consultation ToConsultation(ConsultationCreateDto dto);
        public static partial void UpdateConsultation(ConsultationUpdateDto dto, Consultation entity);

        // nested short mappings
        public static partial DoctorShortDto ToDoctorShortDto(Doctor doctor);
        public static partial PatientShortDto ToPatientShortDto(Patient patient);
    }
}