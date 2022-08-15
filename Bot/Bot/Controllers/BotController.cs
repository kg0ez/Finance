using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Dto;
using Bot.Helper.Handler;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers
{
    public class BotController
    {
        private readonly IButtonService _buttonService;
        private readonly ICategoryService _categoryType;
        private readonly IOperationService _operationService;

        private MessageHendler messageHendler;

        public BotController(IButtonService buttonService, ICategoryService categoryType,IOperationService operationService,ICurrencyService currencyService)
        {
            _categoryType = categoryType;
            _buttonService = buttonService;
            _operationService = operationService;
            messageHendler = new MessageHendler(_buttonService,_categoryType,_operationService,currencyService);
        }
        public async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await messageHendler.HandleMessage(botClient, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(botClient, update.CallbackQuery);
                return;
            }
        }
        
        public async Task HandleCallbackQuery(ITelegramBotClient botClient,
            CallbackQuery callbackQuery)
        {
            List<CategoryDto> list = _categoryType.Get();

            if (callbackQuery.Data.StartsWith("category_next"))
            {
                await _buttonHendler.NextPage(botClient, callbackQuery, _buttonService);
                return;
            }
            if (callbackQuery.Data.StartsWith("category_back"))
            {
                await _buttonHendler.BackPage(botClient, callbackQuery, _buttonService);
                return;
            }
            foreach (var item in list)
            {
                if (callbackQuery.Data.StartsWith(item.Name))
                {
                    OperationService.CategoryId = item.Id;
                    await botClient.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    "Введите количество заработанных средств, используя тег /m-сумма\n /m-2500",
                    replyMarkup:_buttonService.MenuButtonBack());
                    return;
                }
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }
        private CategoryButtonHendler _buttonHendler = new CategoryButtonHendler();
    }
}

