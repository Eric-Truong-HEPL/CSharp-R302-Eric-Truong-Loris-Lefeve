using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class MedicamentMapper
    {
        public static partial MedicamentDto ToMedicamentDto(Medicament entity);
        public static partial MedicamentShortDto ToMedicamentShortDto(Medicament entity);
        public static partial Medicament ToMedicament(MedicamentCreateDto dto);
        public static partial void UpdateMedicament(MedicamentUpdateDto dto, Medicament entity);
    }
}