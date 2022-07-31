using MyPet.BLL.DTO;
using MyPet.BLL.Models.Chat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyPet.BLL.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatResponseModel>> GetChatsByUserId(string userId);        
        Task<MessageResponseModel> AddMessageToChat(MessageDTO message);
        Task<IEnumerable<MessageResponseModel>> GetChatMessagesById(int id);
    }
}
