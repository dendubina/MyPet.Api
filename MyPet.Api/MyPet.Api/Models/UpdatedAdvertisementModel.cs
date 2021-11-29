using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class UpdatedAdvertisementModel
    {
        [Required]
        public int AdId { get; set; }
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

        public IFormFile Image { get; set; }
    }
}
