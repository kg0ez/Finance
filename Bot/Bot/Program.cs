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

var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IButtonService, ButtonService>()
            .AddSingleton<IErrorService,ErrorService>()
            .AddSingleton<ICategoryService, CategoryService>()
            .AddSingleton<IOperationService,OperationService>()
            .AddSingleton<ICurrencyService,CurrencyService>()
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

categoryService.Mapper = mapper;

var botController = new BotController(buttonService,categoryService,operationService,currencyService);

var botClient = new TelegramBotClient("5588306325:AAGxT9g--Yggo0qkaHzNsYa1rDmDh3SoNvc");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};

botClient.StartReceiving(
    botController.HandleUpdatesAsync,
    errorService.HandleError,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();
Console.WriteLine(me.Username + " is working");
Console.ReadLine();

cts.Cancel();
