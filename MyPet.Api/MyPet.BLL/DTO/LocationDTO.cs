using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.DTO
{
    public class LocationDTO
    {
        public int Id { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public virtual PetDTO Pet { get; set; }
        public int PetId { get; set; }
    }
}
