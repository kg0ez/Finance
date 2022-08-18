using Bot.BusinessLogic.Helper.GoogleAPI;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Models.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class DriveService : IDriveService
    {
        private const string DEFAULT_FILE_ID = "1f6rBrGRmrKehbeav19lgB1swyIkrhCjfjHkjDta52Ww";
        private readonly Google.Apis.Drive.v3.DriveService _driveService;

        private readonly IUserService _userService;
        private readonly ApplicationContext _context;

        public DriveService(IUserService userService,ApplicationContext context)
        {
            _context = context;
            _userService = userService;
            _driveService = new Google.Apis.Drive.v3.DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = APIInitializer.Credentials,
                ApplicationName = APIInitializer.ApplicationName
            });
        }
        public bool CreateTable(Message message)
        {
            try
            {
                var file = new Google.Apis.Drive.v3.Data.File();
                file.Name = "☕️PersonalFinanceTracker";

                FilesResource.CopyRequest copyRequest = _driveService.Files.Copy(file, DEFAULT_FILE_ID);

                var copiedFile = copyRequest.Execute();

                GetPermission(message.Text, copiedFile.Id);

                var user = _userService.Get(message.From.Username);
                user.SpeadsheetId = copiedFile.Id;

                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void GetPermission(string gmail, string fileId)
        {
            try
            {
                Permission permission = new Permission();
                permission.EmailAddress = gmail;
                permission.Type = "user";
                permission.Role = "writer";

                PermissionsResource.CreateRequest createPermRequest = _driveService.Permissions
                    .Create(permission, fileId);

                createPermRequest.Execute();
            }
            catch (Exception){}
        }
        

    }
}
