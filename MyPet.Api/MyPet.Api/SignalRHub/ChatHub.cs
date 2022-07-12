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

    }
}
