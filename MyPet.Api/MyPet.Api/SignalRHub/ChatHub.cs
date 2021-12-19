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
            /* var id = Context.User.Claims;
             var userId = Context.User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;            

             var context = Context.GetHttpContext();*/

            // var userid = Context.UserIdentifier;

          //  IReadOnlyList<string> userIds = new IReadOnlyList<string>();

            if (Context.UserIdentifier != message.ToUserId) // если получатель и текущий пользователь не совпадают
                await Clients.User(message.ToUserId).ReceiveMessage(message);

           // await Clients.All.ReceiveMessage(message);
        }
    }
}
