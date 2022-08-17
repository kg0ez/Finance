
using Bot.Common.Enums;
using Bot.Models.Models;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface ISheetService
    {
        void AddDischarge(Operation operation);
        void AddIncome(Operation operation);
        string GetFileLink(string userName);
        void AddCellData(Message message, OperationType type);
    }
}
