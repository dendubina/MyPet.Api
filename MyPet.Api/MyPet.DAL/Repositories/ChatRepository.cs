using Microsoft.EntityFrameworkCore;
using MyPet.DAL.EF;
using MyPet.DAL.Entities.Chat;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return await context.Chats
                .Where(x => x.FirstUserId == userId || x.SecondUserId == userId)
                .ToListAsync();
        }

        public async Task AddMessageToChat(Message message, int? chatId)
        {
            
           if(chatId != null)
            {
                var chat = await context.Chats.FindAsync(chatId);

                chat.Messages.Add(message);
                await context.SaveChangesAsync();
            }
            else
            {
                var chat = await context.Chats
                    .Where(x => x.FirstUserId == message.FromUserId && x.SecondUserId == message.ToUserId
                           || x.FirstUserId == message.ToUserId && x.SecondUserId == message.FromUserId)
                    .FirstOrDefaultAsync();
                if(chat != null)
                {
                    chat.Messages.Add(message);
                    await context.SaveChangesAsync();
                }
                else
                {
                    Chat newChat = new Chat
                    {
                        FirstUserId = message.FromUserId,
                        SecondUserId = message.ToUserId,
                        Messages = new List<Message> { message },
                    };

                   await context.Chats.AddAsync(newChat);
                   await context.SaveChangesAsync();
                }
            }

        }

        public override Task<Chat> Update(int id, Chat entity)
        {
            throw new NotImplementedException();
        }
    }
}
