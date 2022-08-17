using System;
using AutoMapper;
using Bot.Common.Dto;
using Bot.Common.Enums;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface ICategoryService
	{
		List<CategoryDto> Get(int type);
		void Add(Message message, OperationType type);
		bool IsExist(Message message, OperationType type);
		IMapper Mapper { get; set; }
		List<CategoryDto> Get();
	}
}

