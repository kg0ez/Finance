using Bot.BusinessLogic.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace Bot.BusinessLogic.Services.Implementations
{
	public class ErrorService : IErrorService
    {
        public Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}

