using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class OrdonnanceLigneMapper
    {
        public static partial OrdonnanceLigneDto ToOrdonnanceLigneDto(OrdonnanceLigne entity);
        public static partial OrdonnanceLigne ToOrdonnanceLigne(OrdonnanceLigneCreateDto dto);
        public static partial void UpdateOrdonnanceLigne(OrdonnanceLigneUpdateDto dto, OrdonnanceLigne entity);

        // nested
        public static partial MedicamentShortDto ToMedicamentShortDto(Medicament medicament);
        // collection mappings to help Mapperly generate nested list converters
        public static partial System.Collections.Generic.List<OrdonnanceLigneDto> ToOrdonnanceLigneDtoList(System.Collections.Generic.IEnumerable<OrdonnanceLigne> entities);
        public static partial System.Collections.Generic.List<OrdonnanceLigne> ToOrdonnanceLigneList(System.Collections.Generic.IEnumerable<OrdonnanceLigneCreateDto> dtos);
    }
}