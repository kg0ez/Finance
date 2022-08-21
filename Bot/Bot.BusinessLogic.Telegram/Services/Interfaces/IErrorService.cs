using System;
using Telegram.Bot;

namespace Bot.BusinessLogic.Telegram.Services.Interfaces
{
	public interface IErrorService
	{
		Task HandleError(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken);
	}
}

