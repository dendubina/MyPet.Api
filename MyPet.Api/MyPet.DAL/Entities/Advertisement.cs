using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities
{
    public class Advertisement
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public DateTime PublicationDate { get; set; }        
        public string  Description { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Status { get; set; }
        public virtual List<Image> Images { get; set; }
        public virtual Pet Pet { get; set; }
    }
}
