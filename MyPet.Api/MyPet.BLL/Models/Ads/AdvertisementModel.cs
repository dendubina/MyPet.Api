using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Models.Ads
{
    public class AdvertisementModel //Api/Validators/AdvertisementModelValidator
    {
        public int AdId { get; set; }
        public string PetName { get; set; }       
        public string LocationRegion { get; set; }       
        public string LocationTown { get; set; }        
        public string LocationStreet { get; set; }        
        public string LocationHouse { get; set; }       
        public string Description { get; set; }       
        public string Category { get; set; }       
        public IFormFile Image { get; set; }
    }
}
