using MyPet.DAL.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.DAL.Interfaces
{
   public interface IChatRepository :IBaseRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetChatsByUserId(string userId);
        Task AddMessageToChat(Message message, int? chatId);
    }
}
