using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.SignalRHub.Models
{
    public class ChatMessage
    {       
        public string Message { get; set; }
        public string ToUserId { get; set; }       
    }
}
