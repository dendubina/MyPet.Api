using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MyPet.Api.SignalRHub.Clients;
using MyPet.Api.SignalRHub.Models;

namespace MyPet.Api.SignalRHub
{
    public class ChatHub : Hub<IChatClient>
    {
        [Authorize]
        public async Task SendMessage(ChatMessage message)
        {
            List<string> userIds = new List<string> {
                message.ToUserId,
                Context.UserIdentifier,
            };

            await Clients.Users(userIds).ReceiveMessage(message);
            
        }
    }
}
