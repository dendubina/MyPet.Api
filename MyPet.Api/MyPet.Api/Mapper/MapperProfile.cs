using AutoMapper;
using MyPet.Api.Models;
using MyPet.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AdvertisementDTO, AdvertisementResponseModel>()
               .ForMember(dest => dest.LocationRegion, opt => opt.MapFrom(src => src.Pet.Location.Region))
               .ForMember(dest => dest.LocationStreet, opt => opt.MapFrom(src => src.Pet.Location.Street))
               .ForMember(dest => dest.LocationTown, opt => opt.MapFrom(src => src.Pet.Location.Town))
               .ForMember(dest => dest.LocationHouse, opt => opt.MapFrom(src => src.Pet.Location.House))
               .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name));

            CreateMap<AdvertisementModel, LocationDTO>()
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.LocationRegion))
                .ForMember(dest => dest.Town, opt => opt.MapFrom(src => src.LocationTown))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.LocationStreet))
                .ForMember(dest => dest.House, opt => opt.MapFrom(src => src.LocationHouse));


        }
    }    
}
