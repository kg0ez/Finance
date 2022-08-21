using System.ComponentModel.DataAnnotations;

namespace Bot.Models.Models
{
    public class Permission: BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string FileId { get; set; }
    }
}
