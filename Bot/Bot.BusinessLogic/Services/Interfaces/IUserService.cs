using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        void Create(Message message);
        Models.Models.User Get(string userName);
        Models.Models.User GetByGmail(string userName);
        bool IsUserExist(Message message);
        List<Models.Models.User> GetForNotify();
        bool ToggleNotification(string userName);
        bool ShowNotificationStatus(string userName);
        bool HasTable(Message message);
    }
}
