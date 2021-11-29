using Microsoft.AspNetCore.Http;
using MyPet.Api.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdvertisementModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string PetName { get; set; }
        [Required]
        public string LocationTown { get; set; }
        [Required]
        public string LocationStreet { get; set; }
        [Required]
        public string LocationHouse { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Category { get; set; }

        [Required]
        [ImageExtensionAttribute(ErrorMessage = "Wrong file extension")]
        [MaxImageSizeMBAttribute(ErrorMessage ="Wrong image size")]
        public IFormFile Image { get; set; }
    }
}
