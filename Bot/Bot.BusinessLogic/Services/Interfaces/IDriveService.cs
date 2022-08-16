using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface IDriveService
    {
        bool CreateTable(Message message);
        void GetPermission(string gmail, string fileId);
    }
}
