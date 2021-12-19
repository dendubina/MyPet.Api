using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string Region { get; set; }
        [Required]
        public string Town { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string House { get; set; }
        public virtual Pet Pet { get; set; }
        public int PetId { get; set; }

    }
}
