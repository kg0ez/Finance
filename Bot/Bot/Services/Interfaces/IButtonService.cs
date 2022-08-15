using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Services.Interfaces
{
	public interface IButtonService
	{
		ReplyKeyboardMarkup MenuButton(KeyboardButton[] buttonsFirstRow,
			KeyboardButton[]? buttonsSecondRow = default,
			KeyboardButton[]? buttonsThirdRow = default);
		List<InlineKeyboardButton> CategoryButtons();
		ReplyKeyboardMarkup MenuButtonBack();
	}
}

