using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Models.Chat
{
    public class ChatResponseModel
    {
        public int Id { get; set; }
        public string WithUserId { get; set; }
        public string WithUserName { get; set; }
      //  public int MessagesCount { get; set; }
    }
}
