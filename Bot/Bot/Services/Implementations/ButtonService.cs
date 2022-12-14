using Bot.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Services.Implementations
{
    public class ButtonService : IButtonService
    {
        public ReplyKeyboardMarkup MenuButton(KeyboardButton[] buttonsFirstRow,
            KeyboardButton[]? buttonsSecondRow = default,
            KeyboardButton[]? buttonsThirdRow = default)
        {
            ReplyKeyboardMarkup keyboard = new(buttonsFirstRow);

            if (buttonsThirdRow != default)
            {
                keyboard = new(new[]
                    { buttonsFirstRow,
                        buttonsSecondRow!,
                        buttonsThirdRow!});
            }

            else if (buttonsSecondRow != default)
            {
                keyboard = new(new[]
                    { buttonsFirstRow,
                        buttonsSecondRow!});
            }

            keyboard.ResizeKeyboard = true;
            return keyboard;
        }

        public List<InlineKeyboardButton> CategoryButtons()
        {
            List<InlineKeyboardButton> buttons = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("Назад", "category_back"),
                    InlineKeyboardButton.WithCallbackData("Далее", "category_next"),
                };
            return buttons;
        }
        public List<InlineKeyboardButton> Settings(bool notificationStatus)
        {
            List<InlineKeyboardButton> buttons;
            if (notificationStatus)
            {

                buttons = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("✅ Уведомления бота", "notification"),
                };
                return buttons;
            }
            buttons = new List<InlineKeyboardButton>
                {
                    InlineKeyboardButton.WithCallbackData("⛔ Уведомления бота", "notification"),
                };
            return buttons;

        }
        public ReplyKeyboardMarkup MenuButtonBack()
        {
            ReplyKeyboardMarkup keyboard = new(
                new KeyboardButton[] { "Назад" }
            )
            {
                ResizeKeyboard = true
            };
            return keyboard;
        }
    }
}

