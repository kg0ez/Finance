using System;
using System.Text.Json.Serialization;
using Bot.Common.Enums;

namespace Bot.Models.Models
{
	public class Operation:BaseModel
	{
        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        public decimal Price { get; set; }

        public string NameUser { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}

