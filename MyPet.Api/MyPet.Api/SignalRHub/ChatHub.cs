using Microsoft.AspNetCore.SignalR;
using MyPet.Api.SignalRHub.Clients;

namespace MyPet.Api.SignalRHub
{
    public class ChatHub : Hub<IChatClient>
    {

    }
}
