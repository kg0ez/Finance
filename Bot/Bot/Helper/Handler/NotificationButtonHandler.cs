using Bot.Services.Implementations;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helper.Handler
{
    public class NotificationButtonHandler 
    {
        private readonly IButtonService _buttonService;
        public NotificationButtonHandler(IButtonService buttonService)
        {
            _buttonService = buttonService;
        }
        public async void EditButtons(ITelegramBotClient bot, bool result,CallbackQuery callbackQuery)
        {
            InlineKeyboardMarkup keyboard;
            if (result)
            {
                keyboard = new(_buttonService.Settings(result));

                await bot.EditMessageTextAsync(callbackQuery.Message.Chat.Id,callbackQuery.Message.MessageId, " Уведомления бота включены",
                        replyMarkup: keyboard);
                return;
            }
            keyboard = new(_buttonService.Settings(result));
                await bot.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, " Уведомления бота выключены",
                        replyMarkup: keyboard);
            return;
        }
    }
}
