using System;
using System.ComponentModel.DataAnnotations;

namespace Bot.Models.Models
{
	public class BaseModel
	{
        [Key]		
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}

