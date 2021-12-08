using AutoMapper;
using MyPet.BLL.DTO;
using MyPet.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Advertisement, AdvertisementDTO>();

            CreateMap<Pet, PetDTO>().ReverseMap();

            CreateMap<Location, LocationDTO>().ReverseMap();

            CreateMap<Image, ImageDTO>().ReverseMap();



            /*CreateMap<PetDTO, Pet>();
            CreateMap<LocationDTO, Location>();
            CreateMap<ImageDTO, Image>();*/
        }
    }
}
