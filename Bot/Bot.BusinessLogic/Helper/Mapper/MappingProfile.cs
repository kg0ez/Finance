using AutoMapper;
using Bot.Common.Dto;
using Bot.Models.Models;

namespace Bot.BusinessLogic.Helper.Mapper
{
	public class MappingProfile:Profile
	{
		public MappingProfile()
		{
			CreateMap<Category, CategoryDto>().ReverseMap();
		}
	}
}

