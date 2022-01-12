using AutoMapper;
using MyPet.BLL.DTO;
using MyPet.BLL.Models.Ads;
using MyPet.BLL.Models.Chat;
using MyPet.DAL.Entities;
using MyPet.DAL.Entities.Chat;
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

            CreateMap<Chat, ChatDTO>().ReverseMap();
            CreateMap<Message, MessageDTO>().ReverseMap();
            CreateMap<Message, MessageResponseModel>().ReverseMap();
            CreateMap<MessageDTO, MessageResponseModel>().ReverseMap();

            CreateMap<Message, MessageResponseModel>().ReverseMap();

            CreateMap<AdvertisementModel, Location>()
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.LocationRegion))
                .ForMember(dest => dest.Town, opt => opt.MapFrom(src => src.LocationTown))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.LocationStreet))
                .ForMember(dest => dest.House, opt => opt.MapFrom(src => src.LocationHouse));            
        }
    }
}
