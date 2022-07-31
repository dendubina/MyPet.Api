using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Entities.Chat;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPet.DAL.Repositories
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(AppDbContext context) : base(context)
        {

        }        

        public async Task<IEnumerable<Chat>> GetChatsByUserId(string userId)
        {
            return await DbContext.Chats
                .Where(x => x.FirstUserId == userId || x.SecondUserId == userId)               
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddMessageToChat(Message message, int? chatId)
        {

            if (chatId != null)
            {
                var chat = await DbContext.Chats.FindAsync(chatId);

                chat.Messages.Add(message);
                await DbContext.SaveChangesAsync();
            }
            else
            {
                var chat = await DbContext.Chats
                    .Where(x => x.FirstUserId == message.FromUserId && x.SecondUserId == message.ToUserId
                           || x.FirstUserId == message.ToUserId && x.SecondUserId == message.FromUserId)
                    .FirstOrDefaultAsync();

                if(chat != null)
                {
                    chat.Messages.Add(message);
                    await DbContext.SaveChangesAsync();
                }
                else
                {
                    var newChat = new Chat
                    {
                        FirstUserId = message.FromUserId,
                        SecondUserId = message.ToUserId,
                        Messages = new List<Message> { message },
                    };

                   await DbContext.Chats.AddAsync(newChat);
                   await DbContext.SaveChangesAsync();
                }
            }

        }

        public async Task<IEnumerable<Message>> GetMessagesByChatId(int id)
        {
            var chat = await DbContext.Chats
                .Where(x => x.Id == id)
                .Include(x => x.Messages)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return chat.Messages;
        }

        public override Task<Chat> Update(int id, Chat entity)
        {
            throw new NotSupportedException();
        }
    }
}
