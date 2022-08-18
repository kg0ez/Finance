using Bot.Common.Dto;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Helper.Handler
{
	public class CategoryButtonHendler
	{
        public static int PageCount { get; set; }
        private int _pageNumber { get; set; } = 1;
        public static List<CategoryDto> ListCategory { get; set; }

        public async Task NextPage(ITelegramBotClient bot, CallbackQuery callbackQuery, IButtonService buttonService)
        {
            _pageNumber++;
            if (_pageNumber > PageCount)
            {
                _pageNumber = PageCount;
                return;
            }

            await ChangePage(bot, callbackQuery, buttonService);
            return;
        }
        public async Task BackPage(ITelegramBotClient bot, CallbackQuery callbackQuery, IButtonService buttonService)
        {
            _pageNumber--;
            if (_pageNumber < 1)
            {
                _pageNumber = 1;
                return;
            }

            await ChangePage(bot,callbackQuery,buttonService);
            return;
        }
        private async Task ChangePage(ITelegramBotClient bot, CallbackQuery callbackQuery, IButtonService buttonService)
        {
            List<List<InlineKeyboardButton>> buttonsCategories = new List<List<InlineKeyboardButton>>();

            for (int i = (3 * _pageNumber) - 3; i < 3 * _pageNumber; i++)
                if (i < ListCategory.Count)

            buttonsCategories.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(
                ListCategory[i].Name, ListCategory[i].Name) });

            buttonsCategories.Add(buttonService.CategoryButtons());

            InlineKeyboardMarkup keyboard = new(buttonsCategories);

            await bot.EditMessageTextAsync(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                "Здесь представлены все имеющиеся категории расходов. Если вы не нашли подходящую для " +
                "себя категорию, тогда нажмите кнопку “Назад“ и затем нажмите “Добавить категорию“",
                replyMarkup: keyboard);
            return;
        }
    }
}

