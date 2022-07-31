using System;

namespace MyPet.BLL.Models.Chat
{
    public class MessageResponseModel
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public DateTime SendingDate { get; set; }
        public bool isRead { get; set; }
        public string Text { get; set; }
    }
}
