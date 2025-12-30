using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using KindomHospital.Domain.Entities;
using KindomHospital.Application.DTOs;

namespace KindomHospital.Application.Mappers
{
    [Mapper]
    public static partial class OrdonnanceMapper
    {
        public static partial OrdonnanceDto ToOrdonnanceDto(Ordonnance entity);
        public static partial Ordonnance ToOrdonnance(OrdonnanceCreateDto dto);
        public static partial void UpdateOrdonnance(OrdonnanceUpdateDto dto, Ordonnance entity);

        // nested -> generator utilisera OrdonnanceLigne mappings
        public static partial OrdonnanceLigneDto ToOrdonnanceLigneDto(OrdonnanceLigne entity);
        public static partial OrdonnanceLigne ToOrdonnanceLigne(OrdonnanceLigneCreateDto dto);
    }
}