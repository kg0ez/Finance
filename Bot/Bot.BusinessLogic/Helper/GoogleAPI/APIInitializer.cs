using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.BusinessLogic.Helper.GoogleAPI
{
    public static class APIInitializer
    {
        public static UserCredential Credentials { get; set; }
        public static string ApplicationName = "FinanceTracker"/*Google Sheets API.NET Quickstart*/;
        static APIInitializer()
        {
            string[] Scopes = { SheetsService.Scope.Drive };
            using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                Credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
        }
    }
}
