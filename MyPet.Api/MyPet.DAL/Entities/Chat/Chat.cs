using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities.Chat
{
    public class Chat
    {
        public int Id { get; set; }
        [Required]
        public string FirstUserId { get; set; }
        [Required]
        public string SecondUserId { get; set; }
        public virtual List<Message> Messages { get; set; }
    }
}
