using Bot.BusinessLogic.Services.Interfaces;
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
            Models.Models.User user = new Models.Models.User()
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
                return false;

            return true;
        }

        public Models.Models.User Get(string userName)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.UserName == userName);

            if (user == null)
                return null;

            return user;
        }
        public Models.Models.User GetByGmail(string gmail)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.GMail == gmail);

            if (user == null)
                return null;

            return user;
        }
        public Models.Models.User GetByFirstName(string firstName)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.FirstName == firstName);

            if (user == null)
                return null;

            return user;
        }

        public List<Models.Models.User> GetForNotify()
        {
            var query = _context.Users.Where(x => x.NotificationIsToggle == true);
            var users = query.AsNoTracking().ToList();
            return users;
        }

        public bool ToggleNotification(string userName)
        {
            var user = Get(userName);
            if (user.NotificationIsToggle)
            {
                user.NotificationIsToggle = false;
                return false;
            }
            user.NotificationIsToggle = true;
            _context.SaveChanges();
            return true;
        }
        public bool ShowNotificationStatus(string userName)
        {
            var user = Get(userName);
            return user.NotificationIsToggle;
        }
        public bool HasTable(Message message)
        {
            Models.Models.User? user = Get(message.From.Username);
            if (user!= null & user?.SpeadsheetId != null)
            {
                return true;
            }
            Models.Models.User? userByLogin = GetByFirstName(message.From.FirstName);
            if (user != null & user?.SpeadsheetId != null)
            {
                return true;
            }
            return false;
        }
    }
}
