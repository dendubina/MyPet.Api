using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }       
        public string FromUserId { get; set; }        
        public string ToUserId { get; set; }
        public string ToUserName { get; set; }        
        public string FromUserName { get; set; }
        public DateTime SendingDate { get; set; }        
        public bool isRead { get; set; }       
        public string Text { get; set; }        
        public int? ChatId { get; set; }
    }
}
