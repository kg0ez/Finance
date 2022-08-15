using Bot.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Services.Implementations
{
	public class ButtonService: IButtonService
	{
        public ReplyKeyboardMarkup MenuButton(KeyboardButton[] buttonsFirstRow,
            KeyboardButton[]? buttonsSecondRow = default,
            KeyboardButton[]? buttonsThirdRow = default)
        {
            ReplyKeyboardMarkup keyboard;
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
            else
                keyboard = new(buttonsFirstRow);
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
        public ReplyKeyboardMarkup MenuButtonBack()
        {
            ReplyKeyboardMarkup keyboard = new(
                new KeyboardButton[] {  "Назад" }
            )
            {
                ResizeKeyboard = true
            };
            return keyboard;
        }
    }
}

