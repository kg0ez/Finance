using Google.Apis.Drive.v3.Data;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface IDriveService
    {
        bool CreateTable(Message message);
        void SetPermission(string gmail, string fileId);
        Permission SetPermission(Message message);
        bool HasPermission(Message message);
        bool DeletePermission(Message message);
    }
}
