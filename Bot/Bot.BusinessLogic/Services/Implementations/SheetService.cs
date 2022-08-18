using Bot.BusinessLogic.Helper.GoogleAPI;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Enums;
using Bot.Models.Models;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class SheetService : ISheetService
    {
        private readonly SheetsService _sheetsService;
        private readonly IUserService _userService;
        private const string FIRST_SHEET_NAME = "Расходы";
        private const string SECOND_SHEET_NAME = "Доходы";
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

            string range = $"{FIRST_SHEET_NAME}!A:D";
            var requestBody = new ValueRange();
            var objectList = new List<object> { $"{DateTime.Now}", $"{operation.Category.Name}", $"{operation.Price},", $"{user.FirstName}" };
            requestBody.Values = new List<IList<object>> { objectList };
            var appendRequest = _sheetsService.Spreadsheets.Values
                .Append(requestBody, SpreadSheet.SpreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }
        public void AddIncome(Operation operation)
        {
            var user = _userService.Get(operation.NameUser);
            var SpreadSheet = GetSpreadsheet(user.SpeadsheetId);

            string range = $"{SECOND_SHEET_NAME}!A:D";
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
        private int GetColumnIndex(string SpreadsheetId, string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest getValueRequest = _sheetsService.Spreadsheets.Values.Get(SpreadsheetId, range);
            var getValueResponse = getValueRequest.Execute();
            var columnIndex = getValueResponse.Values[2].Count;
            return columnIndex;
        }
        private string ColumnIndexToLetter(int inputColumnIndex)
        {
            string outputColumnName = "";
            int Base = 26;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int TempNumber = inputColumnIndex;
            while (TempNumber > 0)
            {
                int position = TempNumber % Base;
                outputColumnName = (position == 0 ? "Z" : chars[position > 0 ? position - 1 : 0] + outputColumnName);
                TempNumber = (TempNumber - 1) / Base;
            }
            return outputColumnName;
        }
        private List<RowData> GetRowData(string categoryName, int columnIndex)
        {
            Border border = new Border()
            {
                Style = "Solid"
            };
            Borders borders = new Borders()
            {
                Bottom = border,
                Left = border,
                Right = border,
                Top = border
            };
            
            ColorStyle colorStyle = new ColorStyle()
            {
                RgbColor = new Color()
                {
                    Alpha = (float)1.0,
                    Blue = 44,
                    Green = 16,
                    Red = 38,
                }
            };
            CellFormat cellFormat = new CellFormat()
            {
                Borders = borders,
            };
            CellFormat headerCellFormat = new CellFormat()
            {
                TextFormat = new TextFormat()
                {
                    Bold = true
                },
                VerticalAlignment = "MIDDLE",
                HorizontalAlignment = "CENTER",
                Borders = borders,
                BackgroundColorStyle = colorStyle
            };

            var headerCellData = new CellData()
            {
                UserEnteredValue = new ExtendedValue() { StringValue = categoryName },
                UserEnteredFormat = headerCellFormat
            };

            var rowDataList = new List<RowData>()
            {
                new RowData() {
                    Values = new List<CellData>()
                    {
                        headerCellData
                    }
                }
            };
            for (int i = 0; i < 12; i++)
            {
                var rowData = new RowData()
                {
                    Values = new List<CellData>()
                    {
                        new CellData()
                        {
                            UserEnteredValue = new ExtendedValue()
                            {
                                FormulaValue = $"=СУММПРОИЗВ($C$17:$C1727;--(МЕСЯЦ($A$17:$A1727)=$A{i+4});--(($B$17:$B1727)={ColumnIndexToLetter(columnIndex+1)}$3);--(ГОД($A$17:$A1727)=ГОД($A$2)))"
                            },
                            UserEnteredFormat = cellFormat
                        }
                    }
                };
                rowDataList.Add(rowData);
            }
            return rowDataList;
        }
        private void AddColumn(string spreadsheetId, int? sheetId)
        {
            AppendDimensionRequest appendDimensionRequestBody = new AppendDimensionRequest()
            {
                Dimension = "Columns",
                Length = 1,
                SheetId = sheetId
            };
            Request request = new Request()
            {
                AppendDimension = appendDimensionRequestBody,
            };
            IList<Request> requests = new List<Request> { request };
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheet = new BatchUpdateSpreadsheetRequest()
            {
                Requests = requests,
                IncludeSpreadsheetInResponse = true,
                ResponseIncludeGridData = true,
            };

            SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = _sheetsService.Spreadsheets
                .BatchUpdate(batchUpdateSpreadsheet, spreadsheetId);
            var batchUpdateResponse = batchUpdateRequest.Execute();
        }
        public void AddCellData(Message message, OperationType type)
        {
            var categoryName = message.Text.Substring(4);
            var user = _userService.Get(message.From.Username);
            var spreadSheet = GetSpreadsheet(user.SpeadsheetId);
            int? SheetId = spreadSheet.Sheets[(int)type].Properties.SheetId;
            var range = type == 0 ? FIRST_SHEET_NAME : SECOND_SHEET_NAME;
            try
            {
                UpdateCellsRequest updateCellsRequestBody = new UpdateCellsRequest()
                {
                    Start = new GridCoordinate()
                    {
                        SheetId = SheetId,
                        ColumnIndex = GetColumnIndex(spreadSheet.SpreadsheetId, range),
                        RowIndex = 2
                    },
                    Rows = GetRowData(categoryName, GetColumnIndex(spreadSheet.SpreadsheetId, range)),
                    Fields = "UserEnteredValue,UserEnteredFormat",
                };
                Request request = new Request()
                {
                    UpdateCells = updateCellsRequestBody
                };
                IList<Request> requests = new List<Request> { request };
                BatchUpdateSpreadsheetRequest batchUpdateSpreadsheet = new BatchUpdateSpreadsheetRequest()
                {
                    Requests = requests,
                    IncludeSpreadsheetInResponse = true,
                    ResponseIncludeGridData = true,
                };
                SpreadsheetsResource.BatchUpdateRequest batchUpdateRequest = _sheetsService.Spreadsheets
                    .BatchUpdate(batchUpdateSpreadsheet, spreadSheet.SpreadsheetId);
                var batchUpdateResponse = batchUpdateRequest.Execute();
                UpdateCells(SheetId,spreadSheet.SpreadsheetId,GetColumnIndex(spreadSheet.SpreadsheetId,range),range);
            }
            catch (Exception)
            {
                AddColumn(spreadSheet.SpreadsheetId, SheetId);
                AddCellData(message, type);
            }
        }
        private void UpdateCells(int? sheetId, string spreadsheetId, int columnIndex,string range)
        {
            var updateCellsRequest = new Request()
            {
                SetDataValidation = new SetDataValidationRequest()
                {
                    Range = new GridRange()
                    {
                        SheetId = sheetId,
                        StartRowIndex = 16,
                        StartColumnIndex = 1,
                        EndColumnIndex = 2
                    },
                    Rule = new DataValidationRule()
                    {
                        Condition = new BooleanCondition()
                        {
                            Type = "ONE_OF_RANGE",
                            Values = new List<ConditionValue>()
                            {
                                new ConditionValue()
                                        {
                                            UserEnteredValue = $"={range}!D3:{ColumnIndexToLetter(columnIndex)}3",
                                        }
                            }
                        },
                        InputMessage = "Select an Option",
                        ShowCustomUi = true,
                        Strict = true
                    }
                }
            };
            var requestBody = new BatchUpdateSpreadsheetRequest();
            var requests = new List<Request>()
            {
                updateCellsRequest
            };
            requestBody.Requests = requests;
            var batchRequest = _sheetsService.Spreadsheets.BatchUpdate(requestBody, spreadsheetId);
            batchRequest.Execute();
        }
    }
}
