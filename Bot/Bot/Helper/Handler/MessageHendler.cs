using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common;
using Bot.Common.Dto;
using Bot.Common.Enums;
using Bot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Bot.Helper.Handler
{
    public class MessageHendler
    {
        private readonly IButtonService _buttonService;
        private readonly ICategoryService _categoryService;
        private readonly IOperationService _operationService;
        private readonly ICurrencyService _currencyService;
        private readonly ISheetService _sheetService;
        private readonly IDriveService _driveService;
        private readonly IUserService _userService;

        private ReplyKeyboardMarkup _mainKeyboard { get; }
        private ReplyKeyboardMarkup _accountingKeyboard { get; }

        private bool _isActiveIncome { get; set; }

        public MessageHendler(IButtonService buttonService,
            ICategoryService categoryType,
            IOperationService operationService,
            ICurrencyService currencyService,
            ISheetService sheetService,
            IDriveService driveService,
            IUserService userService)
        {
            _sheetService = sheetService;
            _buttonService = buttonService;
            _categoryService = categoryType;
            _operationService = operationService;
            _currencyService = currencyService;
            _driveService = driveService;
            _userService = userService;

            _mainKeyboard = _buttonService.MenuButton(
                    new KeyboardButton[] { "💸 Добавить расходы", "💰 Добавить доходы" },
                new KeyboardButton[] { "📄 Моя таблица", "👥 Совместный учет" },
                new KeyboardButton[] { "⚙️ Настройки" });

            _accountingKeyboard = _buttonService.MenuButton(
                    new KeyboardButton[] { "Выбрать категорию", "Добавить категорию" },
                    new KeyboardButton[] { "На главную" }
                    );
        }

        public async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == "/start" || message.Text == "На главную")
            {
                if (message.Text == "На главную")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Вы вернулись на главную страницу", replyMarkup: _mainKeyboard);

                    _isActiveIncome = false;
                    return;
                }
                if (_userService.IsUserExist(message))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать! Я буду вести учёт ваших доходов и расходов!",
                        replyMarkup: _mainKeyboard);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать! Я буду вести учёт ваших доходов и расходов!" +
                        " Введите свой адрес почты gmail.");

                _userService.Create(message);
                return;
            }

            if (message.Text.Contains("gmail"))
            {
                var isCreated = _driveService.CreateTable(message);

                if (isCreated)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ваша таблица для учёта данных успешно создана!",
                        replyMarkup: _mainKeyboard);
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat.Id, "Неверный ввод, повторите попытку.");
                return;
            }
            if (message.Text.Contains("📄 Моя таблица"))
            {
                var fileLink = _sheetService.GetFileLink(message.From.Username);

                await botClient.SendTextMessageAsync(message.Chat.Id, $"🔗 Ссылка на вашу таблицу: {fileLink}");
                return;
            }
            if (message.Text == "💰 Добавить доходы" || message.Text == "Назад" || message.Text == "💸 Добавить расходы")
            {
                string text = "Можете выбрать уже существующую категорию, нажав “Выбрать категорию“, или добавить, нажав “Добавить категорию“.";
                _isActiveIncome = false;

                if (message.Text == "💰 Добавить доходы")
                {
                    text = "Для ввода доходов, пожалуйста, выберите категорию доходов или создайте свою";
                    _isActiveIncome = true;
                }
                else if (message.Text == "💸 Добавить расходы")
                    text = "Для ввода расходов, пожалуйста, выберите категорию расходов или создайте свою";

                var user = _userService.Get(message.From.Username).Id;
                ListOfSelectedIndexes.SelectedIndexes.Remove(user);
                await botClient.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: _accountingKeyboard);
                return;
            }
            if (message.Text == "Выбрать категорию")
            {
                int typeCategory = 0;

                if (_isActiveIncome)
                    typeCategory = 1;

                List<CategoryDto> categoriesDto = _categoryService.GetAllByType(typeCategory,message.From.Username);

                CategoryButtonHendler.PageCount = Convert.ToInt32(Math.Ceiling((double)categoriesDto.Count / 3));
                CategoryButtonHendler.ListCategory = categoriesDto;

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();

                for (int i = 0; i < 3; i++)
                    buttons.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(
                        categoriesDto[i].Name, categoriesDto[i].Name)});

                buttons.Add(_buttonService.CategoryButtons());
                InlineKeyboardMarkup keyboard = new(buttons);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Здесь представлены все имеющиеся категории доходов. Если вы не нашли подходящую для себя категорию, то нажмите кнопку “Назад“ и затем нажмите “Добавить категорию“", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Добавить категорию")
            {
                string typeOperation = "расходов";

                if (_isActiveIncome)
                    typeOperation = "доходов";

                await botClient.SendTextMessageAsync(message.Chat.Id,
                    $"Здесь вы можете создать категорию {typeOperation}. Название может быть написано на любом языке, включать цифры, символы и смайлики“. \n Для добавления используйте тег /ct-категория",
                    replyMarkup: _buttonService.MenuButtonBack());
                return;
            }
            if (message.Text.Length > 3 && message.Text.Substring(0, 3) == "/m-")
            {
                try
                {
                    _operationService.Price = Convert.ToDecimal(message.Text.Substring(3));

                    ReplyKeyboardMarkup keyboardMarkup = _buttonService.MenuButton(
                        new KeyboardButton[] { "$", "€" },
                        new KeyboardButton[] { "Назад", "Добавить" });

                    await botClient.SendTextMessageAsync(message.Chat.Id, "BYN является валютой по умолчанию, если вы ввели данные в BYN, нажмите “Добавить“. При необходимости " +
                        "конвертирование выберите “$“ или “€“", replyMarkup: keyboardMarkup);
                }
                catch (Exception) { await botClient.SendTextMessageAsync(message.Chat.Id, "Неверный ввод"); }
                return;
            }
            if (message.Text == "Добавить" || message.Text == "$" || message.Text == "€")
            {
                decimal exchangeRate = 1;

                if (message.Text == "$" || message.Text == "€")
                    exchangeRate = _currencyService.Get(message.Text);

                if (_operationService.Price == 0)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка");
                    return;
                }

                try
                {
                    decimal value = _operationService.Price * exchangeRate;

                    _operationService.Add(value, message.From.Username);

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Успешно выполнено",
                        replyMarkup: _mainKeyboard);
                }
                catch (Exception) { }
                return;
            }
            if (message.Text.Length > 4 && message.Text.Substring(0, 4) == "/ct-")
            {
                OperationType type = OperationType.Discharge;

                if (_isActiveIncome)
                    type = OperationType.Income;
                
                if (_categoryService.IsExist(message, type))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Категория не была добавлена", replyMarkup: _mainKeyboard);
                    return;
                }

                _categoryService.Add(message, type);

                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Введите количество потраченных/заработанных средств, используя тег /m-сумма");
                return;
            }
            if (message.Text == "⚙️ Настройки")
            {
                InlineKeyboardMarkup keyboard = new(_buttonService.Settings());
                await botClient.SendTextMessageAsync(message.Chat.Id, "Основные настройки бота",
                        replyMarkup: keyboard);
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Команда: " + message.Text +
                " не найдена");
        }
    }
}

