﻿using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext _context;
        public UserService(ApplicationContext context)
        {
            _context =context;
        }
        public void Create(Message message)
        {
                Bot.Models.Models.User user = new Bot.Models.Models.User()
                {
                    ChatId = message.Chat.Id,
                    UserName = message.From.Username,
                    FirstName = message.From.FirstName,
                    LastName = message.From.LastName,
                };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public bool IsUserExist(Message message)
        {
            var user = _context.Users
                .SingleOrDefault(x => x.UserName == message.From.Username);
            if (user ==null)
            {
                return false;
            }
            return true;
        }
        public Bot.Models.Models.User Get(string userName)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserName == userName);
            if (user != null)
                return user;
            else
                return null;
        }
    }
}
