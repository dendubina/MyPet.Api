using MyPet.BLL.DTO;
using MyPet.BLL.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
