using AutoMapper;
using MyPet.Api.Models;
using MyPet.Api.Models.EmailModels;
using MyPet.BLL.DTO;
using MyPet.BLL.Models.EmailModels;
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
               .ForMember(dest => dest.LocationRegion, opt => opt.MapFrom(src => src.Pet.Location.Region))
               .ForMember(dest => dest.LocationStreet, opt => opt.MapFrom(src => src.Pet.Location.Street))
               .ForMember(dest => dest.LocationTown, opt => opt.MapFrom(src => src.Pet.Location.Town))
               .ForMember(dest => dest.LocationHouse, opt => opt.MapFrom(src => src.Pet.Location.House))
               .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name));

            CreateMap<EmailConfiguration, EmailConfig>();
        }
    }
}
