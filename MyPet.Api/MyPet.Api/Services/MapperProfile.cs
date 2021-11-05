using AutoMapper;
using MyPet.Api.Models;
using MyPet.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AdvertisementDTO, AdvertisementResponseModel>()
               .ForMember(dest => dest.LocationStreet, opt => opt.MapFrom(src => src.Pet.Location.Street))
               .ForMember(dest => dest.LocationTown, opt => opt.MapFrom(src => src.Pet.Location.Town))
               .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name));

            
        }
    }
}
