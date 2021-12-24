using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Models.Chat
{
    public class MessageResponseModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public DateTime SendingDate { get; set; }
        public bool isRead { get; set; }
        public string Text { get; set; }
    }
}
