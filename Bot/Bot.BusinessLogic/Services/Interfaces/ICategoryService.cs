using System;
using AutoMapper;
using Bot.Common.Dto;
using Bot.Common.Enums;

namespace Bot.BusinessLogic.Services.Interfaces
{
	public interface ICategoryService
	{
		List<CategoryDto> Get(int type);
		int Add(string category, OperationType type);
        IMapper Mapper { get; set; }
		List<CategoryDto> Get();
	}
}

