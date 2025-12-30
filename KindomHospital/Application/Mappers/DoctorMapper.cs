using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class DoctorMapper
    {
        public static partial DoctorDto ToDoctorDto(Doctor entity);
        public static partial Doctor ToDoctor(DoctorCreateDto dto);
        public static partial void UpdateDoctor(DoctorUpdateDto dto, Doctor entity);

        // nested
        public static partial SpecialtyShortDto ToSpecialtyShortDto(Specialty specialty);
    }
}