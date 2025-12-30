using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class PatientMapper
    {
        public static partial PatientDto ToPatientDto(Patient entity);
        public static partial Patient ToPatient(PatientCreateDto dto);
        public static partial void UpdatePatient(PatientUpdateDto dto, Patient entity);
    }
}