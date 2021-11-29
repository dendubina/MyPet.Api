using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.DTO
{
    public class AdvertisementDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime PublicationDate { get; set; }       
        public string Description { get; set; }
        public string Category { get; set; }
        public List<ImageDTO> Images { get; set; }
        public PetDTO Pet { get; set; }
    }
}
