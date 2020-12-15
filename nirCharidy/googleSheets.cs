using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.IO;
using System.Windows.Forms;

namespace nirCharidy
{
    public class googleSheets
    {
        public string spreadsheetId = "1diFCLSSaEX3XICDMMp5rmjYYAiLDXD3Z5z1E4JC9CCU";

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        private string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private string ApplicationName = "Nir Charidy";
        private UserCredential credential;
        SheetsService service;

        public bool Intialize()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true)).Result;
                // Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return true;
        }

        public  IList<IList<Object>> getValues(string sheetName,string rangee)
        {
            bool s = Intialize();

            rangee = "!" + rangee;
            // Define request parameters.

            String range = sheetName+rangee;
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            return values;
            
        }

        public List<string> sheetsNames()
        {
            bool s = Intialize();            

            var sheet_metadata = service.Spreadsheets.Get(spreadsheetId).Execute();
            IList<Sheet> sheets = sheet_metadata.Sheets;

            List<string> titles = new List<string>();

            for (int i = 0; i < sheets.Count; i++)
                titles.Add(sheets[i].Properties.Title);

            return titles;
        }
    }
}
