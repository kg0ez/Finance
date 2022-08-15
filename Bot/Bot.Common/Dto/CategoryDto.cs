using System;
using Bot.Common.Enums;

namespace Bot.Common.Dto
{
	public class CategoryDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public OperationType Type { get; set; }
	}
}

