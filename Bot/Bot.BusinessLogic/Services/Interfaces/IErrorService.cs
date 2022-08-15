using Telegram.Bot;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IErrorService
	{
		Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
	}
}

