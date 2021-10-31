using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdvertisementModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }        
        public string PetName { get; set; }
        public string Description { get; set; }        
        public List<IFormFile> Images { get; set; }
    }
}
