using System;
namespace Bot.Models.Models
{
	public class User:BaseModel
	{
        public int ChatId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Category> Categories { get; set; }
    }
}

