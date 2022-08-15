using System;
namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IOperationService
	{
		void Add(decimal price, string name = null);
		static int CategoryId { get; set; }
		decimal Price { get; set; }
	}
}

