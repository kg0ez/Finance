using Bot.BusinessLogic.Helper.GoogleAPI;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Models;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class SheetService : ISheetService
    {
        private readonly SheetsService _sheetsService;
        private readonly IUserService _userService;
        private const string FIRST_SHEET_NAME = "Расходы!";
        private const string SECOND_SHEET_NAME = "Доходы!";
        public SheetService(IUserService userService)
        {
            _userService = userService;
            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = APIInitializer.Credentials,
                ApplicationName = APIInitializer.ApplicationName
            });
        }
        //Нужен рефакторинг
        public void AddDischarge(Operation operation) 
        {
            var user = _userService.Get(operation.NameUser);
            var SpreadSheet = GetSpreadsheet(user.SpeadsheetId);

            string range = $"{FIRST_SHEET_NAME}A:D";
            var requestBody = new ValueRange();
            var objectList = new List<object> { $"{DateTime.Now}", $"{operation.Category.Name}",$"{operation.Price},",$"{user.FirstName}" };
            requestBody.Values = new List<IList<object>> { objectList};
            var appendRequest = _sheetsService.Spreadsheets.Values
                .Append(requestBody, SpreadSheet.SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }

        public void AddIncome(Operation operation)
        {
            var user = _userService.Get(operation.NameUser);
            var SpreadSheet = GetSpreadsheet(user.SpeadsheetId);
            
            string range = $"{SECOND_SHEET_NAME}A:D";
            var requestBody = new ValueRange();
            var objectList = new List<object> { $"{DateTime.Now}", $"{operation.Category.Name}", $"{operation.Price},", $"{user.FirstName}" };
            requestBody.Values = new List<IList<object>> { objectList };
            var appendRequest = _sheetsService.Spreadsheets.Values
                .Append(requestBody, SpreadSheet.SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }
        public Spreadsheet GetSpreadsheet(string spreadSheetId)
        {
            SpreadsheetsResource.GetRequest getRequest = _sheetsService.Spreadsheets.Get(spreadSheetId);
            var getResponse = getRequest.Execute();
            return getResponse;
        }
        public string GetFileLink(string userName)
        {
            var user = _userService.Get(userName);
            var fileId = user.SpeadsheetId;
            SpreadsheetsResource.GetRequest getFileRequest = _sheetsService.Spreadsheets.Get(fileId);
            var getFileResponse = getFileRequest.Execute();
            return getFileResponse.SpreadsheetUrl;

        }
    }
}
