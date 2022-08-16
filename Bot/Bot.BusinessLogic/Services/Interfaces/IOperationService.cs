using Bot.Common.Enums;
using System;
namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface IOperationService
	{
		void Add(decimal price,string name);
		static int CategoryId { get; set; }
		decimal Price { get; set; }
		//void Get();
	}
}

