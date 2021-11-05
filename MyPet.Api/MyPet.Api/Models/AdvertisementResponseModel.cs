using MyPet.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Models
{
    public class AdvertisementResponseModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Description { get; set; }
        public List<ImageDTO> Images { get; set; }


        public string PetName { get; set; }
        public string LocationTown { get; set; }
        public string LocationStreet { get; set; }
    }
}
