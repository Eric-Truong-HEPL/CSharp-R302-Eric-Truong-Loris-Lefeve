using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class SpecialtyMapper
    {
        public static partial SpecialtyDto ToSpecialtyDto(Specialty entity);
        public static partial SpecialtyShortDto ToSpecialtyShortDto(Specialty entity);
        public static partial Specialty ToSpecialty(SpecialtyCreateDto dto);
        public static partial void UpdateSpecialty(SpecialtyUpdateDto dto, Specialty entity);
    }
}