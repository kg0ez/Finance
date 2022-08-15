using System;
using System.Text.Json.Serialization;
using Bot.Common.Enums;

namespace Bot.Models.Models
{
	public class Category
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public OperationType Type { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
        public List<Operation> Operations { get; set; }
    }
}

