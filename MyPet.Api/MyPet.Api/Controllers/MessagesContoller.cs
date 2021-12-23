using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyPet.Api.SignalRHub;
using MyPet.Api.SignalRHub.Clients;
using MyPet.Api.SignalRHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MessagesContoller : ControllerBase
    {

        private readonly IHubContext<ChatHub, IChatClient> chatHub;

        public MessagesContoller(IHubContext<ChatHub, IChatClient> chatHub)
        {
            this.chatHub = chatHub;
        }


        [HttpPost]
        public async Task SendMessage(ChatMessage message)
        {
            List<string> userIds = new List<string> {
                message.ToUserId,               
            };

            await chatHub.Clients.Users(userIds).ReceiveMessage(message);
        }
    }
}
