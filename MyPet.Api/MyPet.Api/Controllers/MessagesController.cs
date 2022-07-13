using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyPet.Api.SignalRHub;
using MyPet.Api.SignalRHub.Clients;
using MyPet.Api.SignalRHub.Models;
using MyPet.BLL.DTO;
using MyPet.BLL.Interfaces;
using System.Threading.Tasks;
using MyPet.Api.Extensions;

namespace MyPet.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {

        private readonly IHubContext<ChatHub, IChatClient> chatHub;
        private readonly IChatService chatService;

        public MessagesController(IHubContext<ChatHub, IChatClient> chatHub, IChatService chatService)
        {
            this.chatHub = chatHub;
            this.chatService = chatService;
        }
       

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessage(ChatMessage message)
        {
            var requestingUserId = Request.GetUserId();

            if(message.ToUserId == requestingUserId)
            {
                ModelState.AddModelError("userId", "sender and receiver are the same");
                return ValidationProblem(ModelState);
            }

            MessageDTO messageDto = new MessageDTO
            {
                FromUserId = requestingUserId,
                ToUserId = message.ToUserId,                
                Text = message.Message,                
            };

            var responseModel = await chatService.AddMessageToChat(messageDto);

            string[] usersToReceiveMessage = {
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
            var userId = Request.GetUserId();

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
