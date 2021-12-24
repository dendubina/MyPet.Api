﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyPet.BLL.DTO;
using MyPet.BLL.Exceptions;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.Chat;
using MyPet.DAL.Entities.Chat;
using MyPet.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepo;
        private readonly IMapper mapper;
        private readonly ILogger<ChatService> logger;
        private readonly UserManager<IdentityUser> userManager;

        public ChatService(IMapper mapper, ILogger<ChatService> logger, IChatRepository chatRepository, UserManager<IdentityUser> userManager)
        {
            this.chatRepo = chatRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
        }
        public async Task<MessageResponseModel> AddMessageToChat(MessageDTO message, int? chatId)
        {
            message.isRead = false;
            message.SendingDate = DateTime.Now;

            var messagetoAdd = mapper.Map<Message>(message);

            await chatRepo.AddMessageToChat(messagetoAdd, chatId);

            MessageResponseModel responseModel = new MessageResponseModel
            {
                FromUserId = message.FromUserId,
                ToUserId = message.ToUserId,
                isRead = message.isRead,
                SendingDate = message.SendingDate,
                Text = message.Text,
            };

            return responseModel;
        }

        public async Task<IEnumerable<ChatResponseModel>> GetChatsByUserId(string userId)
        {
            var requestingUser = await userManager.FindByIdAsync(userId);

            if (requestingUser == null)
            {
                logger.LogError($"User with id {userId} not found in database when getting his chats list");
                throw new NotFoundException($"User with id {userId} not found in database");
            }

            var chats = await chatRepo.GetChatsByUserId(userId);
            var resultChats = new List<ChatResponseModel>();

            foreach (var chat in chats)
            {
                IdentityUser secondUser = new IdentityUser();

                if(chat.FirstUserId == requestingUser.Id)
                {
                    secondUser = await userManager.FindByIdAsync(chat.SecondUserId);
                }
                if(chat.SecondUserId == requestingUser.Id)
                {
                    secondUser = await userManager.FindByIdAsync(chat.FirstUserId);
                }

                if(secondUser.Id == null)
                {
                    logger.LogWarning($"User having id '{requestingUser.Id}' has chat with id '{chat.Id}'. Can't found second user.");
                    continue;
                }

                resultChats.Add(new ChatResponseModel
                {
                    Id = chat.Id,
                    WithUserId = secondUser.Id,
                    WithUserName = secondUser.UserName,
                    MessagesCount = chat.Messages.Count(),
                });
            }

            return resultChats;
        }        
    }
}