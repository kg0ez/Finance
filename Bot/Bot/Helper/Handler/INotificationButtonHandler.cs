using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Helper.Handler
{
    public interface INotificationButtonHandler
    {
        void EditButtons(ITelegramBotClient bot, bool result, CallbackQuery callbackQuery);
    }
}
