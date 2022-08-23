using AutoMapper;
using Bot.BusinessLogic.Helper.Mapper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Controllers;
using Bot.Services.Interfaces;
using Bot.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Bot.Models.Data;
using Microsoft.EntityFrameworkCore;
using Bot.Helper.Handler;
using Bot.BusinessLogic.Telegram.Services.Implementations;
using Bot.BusinessLogic.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddTransient<IButtonService, ButtonService>()
            .AddSingleton<IErrorService, ErrorService>()
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<IOperationService, OperationService>()
            .AddSingleton<ICurrencyService, CurrencyService>()
            .AddSingleton<ISheetService, SheetService>()
            .AddSingleton<IDriveService, DriveService>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<CategoryButtonHendler, CategoryButtonHendler>()
            //Server=localhost;Database=BotFinanceTracking;User Id = sa; Password=Valuetech@123;
            //Server=localhost;Database=WebApiDb1;Trusted_Connection=True;TrustServerCertificate=True;
            .AddDbContext<ApplicationContext>(opt => opt.UseSqlServer("Server=localhost;Database=TrackerBot;Trusted_Connection=True;TrustServerCertificate=True;"
            , x => x.MigrationsAssembly("Bot")))
            .BuildServiceProvider();
var mapperConfiguration = new MapperConfiguration(x =>
{
    x.AddProfile<MappingProfile>();
});
mapperConfiguration.AssertConfigurationIsValid();
IMapper mapper = mapperConfiguration.CreateMapper();

var errorService = serviceProvider.GetService<IErrorService>();
var buttonService = serviceProvider.GetService<IButtonService>();
var categoryService = serviceProvider.GetService<ICategoryService>();
var operationService = serviceProvider.GetService<IOperationService>();
var currencyService = serviceProvider.GetService<ICurrencyService>();
var sheetService = serviceProvider.GetService<ISheetService>();
var driveService = serviceProvider.GetService<IDriveService>();
var userService = serviceProvider.GetService<IUserService>();
var categoryButtonHendler = serviceProvider.GetService<CategoryButtonHendler>();

categoryService.Mapper = mapper;

var botController = new BotController(buttonService, categoryService, operationService, currencyService,
    sheetService, driveService, userService, categoryButtonHendler);

var botClient = new TelegramBotClient("5588306325:AAGxT9g--Yggo0qkaHzNsYa1rDmDh3SoNvc");

using var cts = new CancellationTokenSource();
CallbackQuery callback = new CallbackQuery();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
    botController.HandleUpdatesAsync,
    errorService.HandleError,
    receiverOptions,
    cancellationToken: cts.Token);


//daily
TimeSpan day = new TimeSpan(24, 00, 00);
TimeSpan now = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
TimeSpan activationTime = new TimeSpan(8, 0, 0);

TimeSpan timeLeftUntilFirstRun = ((day - now) + activationTime);
if (timeLeftUntilFirstRun.TotalHours > 24)
    timeLeftUntilFirstRun -= new TimeSpan(24, 0, 0);

System.Timers.Timer execute = new System.Timers.Timer();
execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;
execute.Elapsed += EventHandler;

async void EventHandler(object? sender, System.Timers.ElapsedEventArgs e)
{
    var users = userService.GetForNotify();
    foreach (var user in users)
    {
        await botController.NotificationDaily(user.ChatId);
    }
}

execute.Start();

//Last day in month
var tickerTimer = new System.Timers.Timer();
tickerTimer.Start();
tickerTimer.Elapsed += async (o, e) =>
{
    tickerTimer.Interval = 86400000;
    await Task.Run(async () =>
    {
        if (DateTime.Now.Day >= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
        {
            var users = userService.GetForNotify();
            foreach (var user in users)
            {
                await botController.NotificationMonth(user.ChatId);
            }
        }
    });
};

var me = await botClient.GetMeAsync();
Console.WriteLine(me.Username + " is working");
Console.ReadLine();

cts.Cancel();
