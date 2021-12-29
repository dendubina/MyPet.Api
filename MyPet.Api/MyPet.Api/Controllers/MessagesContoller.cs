using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyPet.Api.SignalRHub;
using MyPet.Api.SignalRHub.Clients;
using MyPet.Api.SignalRHub.Models;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MessagesContoller : ControllerBase
    {

        private readonly IHubContext<ChatHub, IChatClient> chatHub;
        private readonly IChatService chatService;

        public MessagesContoller(IHubContext<ChatHub, IChatClient> chatHub, IChatService chatService)
        {
            this.chatHub = chatHub;
            this.chatService = chatService;
        }
       

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage(ChatMessage message)
        {
            var requestingUserId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

            MessageDTO messageDto = new MessageDTO
            {
                FromUserId = requestingUserId,
                ToUserId = message.ToUserId,                
                Text = message.Message,                
            };

            var responseModel = await chatService.AddMessageToChat(messageDto, message.ToChatId);

            List<string> usersToReceiveMessage = new List<string> {
                message.ToUserId,
                requestingUserId,
            };

            await chatHub.Clients.Users(usersToReceiveMessage).ReceiveMessage(responseModel);
            return Ok(responseModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChatsByUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value;

            var chats = await chatService.GetChatsByUserId(userId);

            return Ok(chats);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetChatMessagesById(int id)
        {
            var messages = await chatService.GetChatMessagesById(id);

            return Ok(messages);
        }
    }
}
