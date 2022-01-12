using FluentValidation;
using Microsoft.AspNetCore.Http;
using MyPet.Api.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdvertisementModel // Validators/AdvertisementModelValidator
    {      
       
       
        public string PetName { get; set; }       
        public string LocationRegion { get; set; }        
        public string LocationTown { get; set; }       
        public string LocationStreet { get; set; }        
        public string LocationHouse { get; set; }        
        public string Description { get; set; }       
        public string Category { get; set; }
        
        [ImageExtensionAttribute(ErrorMessage = "Wrong file extension")]
        [MaxImageSizeMBAttribute(ErrorMessage ="Wrong image size")]
        public IFormFile Image { get; set; }
    }
}
