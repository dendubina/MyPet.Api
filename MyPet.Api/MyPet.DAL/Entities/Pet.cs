using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual Location Location { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public int AdId { get; set; }
        // public int LocationId { get; set; }
    }
}
