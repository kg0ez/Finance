using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common;
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
        private readonly ISheetService _sheetService;
        private readonly IDriveService _driveService;
        private readonly IUserService _userService;
        private ITelegramBotClient _telegramBotClient;
        private NotificationButtonHandler _notificationButtonHandler;

        private MessageHendler messageHendler;
        private CategoryButtonHendler _buttonHendler;

        public BotController(IButtonService buttonService,
            ICategoryService categoryType,
            IOperationService operationService,
            ICurrencyService currencyService,
            ISheetService sheetService,
            IDriveService driveService,
            IUserService userService,
            CategoryButtonHendler categoryButtonHendler)

        {
            _driveService = driveService;
            _categoryType = categoryType;
            _buttonService = buttonService;
            _operationService = operationService;
            _userService = userService;
            _sheetService = sheetService;
            _buttonHendler = categoryButtonHendler;
            //operationService.Get();
            messageHendler = new MessageHendler(_buttonService, _categoryType, _operationService, currencyService, _sheetService, _driveService, _userService);
            _notificationButtonHandler = new NotificationButtonHandler(_buttonService);
        }
        public async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (_telegramBotClient==null)
                    _telegramBotClient = botClient;

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
            catch (Exception)
            {
            }
        }

        public async Task HandleCallbackQuery(ITelegramBotClient botClient,
            CallbackQuery callbackQuery)
        {
            List<CategoryDto> list = _categoryType.GetAll(callbackQuery.From.Username);


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
            if (callbackQuery.Data.StartsWith("notification"))
            {
                var result = _userService.ToggleNotification(callbackQuery.From.Username);

                _notificationButtonHandler.EditButtons(botClient, result, callbackQuery);
                return;
            }
            foreach (var category in list)
            {
                if (callbackQuery.Data.StartsWith(category.Name))
                {
                    ListOfSelectedIndexes.SelectedIndexes.Add(_userService.Get(callbackQuery.From.Username).Id, category.Id);
                    await botClient.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    "Введите сумму, используя тег /m-сумма\n /m-2500",
                    replyMarkup: _buttonService.MenuButtonBack());
                    return;
                }
            }
            await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"You choose with data: {callbackQuery.Data}");
            return;
        }

        public async Task NotificationDaily(long chatId)
        {
            await _telegramBotClient.SendTextMessageAsync(
                    chatId,
                    "🕗 День подходит к концу, не забудьте внести расходы");
        }

        public async Task NotificationMonth(long chatId)
        {
            //выведи инфу, что тут надо в конце месяца
            await _telegramBotClient.SendTextMessageAsync(
                    chatId,
                    "");
        }
    }
}

