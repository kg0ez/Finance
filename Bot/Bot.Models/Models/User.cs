using Google.Apis.Drive.v3.Data;
namespace Bot.Models.Models
{
	public class User:BaseModel
	{
        public long ChatId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? SpeadsheetId { get; set; }
        public string? GMail { get; set; }

        public List<Category> Categories { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}

