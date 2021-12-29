using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Entities.Chat
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string FromUserId { get; set; }
        [Required]
        public string ToUserId { get; set; }
        [Required]
        public string ToUserName { get; set; }
        [Required]
        public string FromUserName { get; set; }
        [Required]
        public DateTime SendingDate { get; set; }
        [Required]
        public bool isRead { get; set; }
        [Required]
        public string Text { get; set; }
        public virtual Chat Chat { get; set; }
        public int ChatId { get; set; }
    }
}
