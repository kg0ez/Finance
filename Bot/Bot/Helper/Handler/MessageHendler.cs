using Bot.BusinessLogic.Helper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
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
        private readonly ICategoryService _categoryType;
        private readonly IOperationService _operationService;
        private readonly ICurrencyService _currencyService;

        private ReplyKeyboardMarkup _mainKeyboard { get; }
        private ReplyKeyboardMarkup _accountingKeyboard { get; }

        private bool _isActiveIncome { get; set; }
        private bool _isActiveExpenses { get; set; }

        public MessageHendler(IButtonService buttonService, ICategoryService categoryType,IOperationService operationService,ICurrencyService currencyService)
        {
            _buttonService = buttonService;
            _categoryType = categoryType;
            _operationService = operationService;
            _currencyService = currencyService;
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
            if (message.Text == "/start" || message.Text =="На главную")
            {
                if (message.Text == "/start")
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать! Я буду вести учёт ваших доходов и расходов! ",
                    replyMarkup: _mainKeyboard);
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Вы вернулись на главную страницу", replyMarkup: _mainKeyboard);
                }
                _isActiveIncome = _isActiveExpenses = false;
                return;
            }
            if(message.Text == "💰 Добавить доходы" || message.Text == "Назад" || message.Text == "💸 Добавить расходы")
            {
                string text = Environment.NewLine;
                if (message.Text == "💰 Добавить доходы")
                {
                    text = "Для ввода доходов, пожалуйста, выберите категорию доходов или создайте свою";
                    _isActiveIncome = true;
                    _isActiveExpenses = false;
                }
                else if (message.Text == "💸 Добавить расходы")
                {
                    text = "Для ввода расходов, пожалуйста, выберите категорию расходов или создайте свою";
                    _isActiveExpenses = true;
                    _isActiveIncome = false;
                }
                else
                    text = "Можете выбрать уже существующую категорию нажав “Выбрать категорию“, или добавить нажав “Добавить категорию“";
                await botClient.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: _accountingKeyboard);
                return;
            }
            if(message.Text == "Выбрать категорию")
            {
                int type = default;
                if (_isActiveIncome)
                    type = 1;
                else
                    type = 0;
                List<CategoryDto> list = _categoryType.Get(type);

                CategoryButtonHendler.PageCount = Convert.ToInt32(Math.Ceiling((double)list.Count / 3));
                CategoryButtonHendler.ListCategory = list;

                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>();
                
                for (int i = 0; i < 3; i++)
                    buttons.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(list[i].Name, list[i].Name) });

                buttons.Add(_buttonService.CategoryButtons());
                InlineKeyboardMarkup keyboard = new(buttons);

                await botClient.SendTextMessageAsync(message.Chat.Id, "Здесь представлены все имеющиеся категории доходов. Если вы не нашли подходящую для себя категорию, то нажмите кнопку “Назад“ и затем нажмите “Добавить категорию“", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Добавить категорию")
            {
                string type = string.Empty;
                if (_isActiveIncome)
                    type = "доходов";
                else
                    type = "расходов";
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Здесь вы можете создать категорию {type}. Название может быть написано на любом языке, включать цифры, символы и смайлики“(здесь, как и везде, смайлик(и)). \n Для добавления используйте тег /ct-категория", replyMarkup: _buttonService.MenuButtonBack());
                return;
            }
            if (message.Text.Length>3 && message.Text.Substring(0, 3)== "/m-")
            {
                //передать имя
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
            if (message.Text=="Добавить" || message.Text == "$" || message.Text == "€")
            {
                decimal coefficient = 1;
                if (message.Text == "$" || message.Text == "€")
                    coefficient = _currencyService.Get(message.Text);
                if (_operationService.Price ==0)
                { 
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка");
                    return;
                }
                try
                {
                    decimal prise = _operationService.Price * coefficient;
                    _operationService.Add(prise);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Успешно выполнено", replyMarkup: _mainKeyboard);
                }
                catch (Exception) { }
                return;
            }
            if (message.Text.Length > 4 && message.Text.Substring(0, 4) == "/ct-")
            {
                string category = message.Text.Substring(4);
                OperationType type;
                if (_isActiveIncome)
                    type = OperationType.Income;
                else
                    type = OperationType.Discharge;
                int id = _categoryType.Add(category, type);
                if (id == 0)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Категория не была добавлена", replyMarkup: _mainKeyboard);
                    return;
                }
                OperationService.CategoryId =id;
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Введите количество заработанных средств, используя тег /m-сумма");
                return;
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Команда: "+message.Text+" не найдена");
        }
    }
}

