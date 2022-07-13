using MyPet.BLL.Models.Chat;
using System.Threading.Tasks;

namespace MyPet.Api.SignalRHub.Clients
{
   public interface IChatClient
    {
        Task ReceiveMessage(MessageResponseModel message);
    }
}
