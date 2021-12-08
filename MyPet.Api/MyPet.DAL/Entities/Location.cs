using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public virtual Pet Pet { get; set; }
        public int PetId { get; set; }

    }
}
