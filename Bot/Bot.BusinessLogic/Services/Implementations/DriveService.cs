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
        private const string DEFAULT_FILE_ID = "1Ri4mKKlDBzL2HzvmtDN_whdpOubRIsA7v5_h4LnEmPk";
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

                var user = _userService.Get(message.From.Username);
                user.SpeadsheetId = copiedFile.Id;
                user.GMail = message.Text;
                _context.SaveChanges();

                SetPermission(message.Text, copiedFile.Id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public void SetPermission(string gmail, string fileId)
        {
            var user = _context.Users.AsNoTracking().FirstOrDefault(x => x.GMail == gmail);
            try
            {
                Permission permissionBody = new Permission();
                permissionBody.EmailAddress = gmail;
                permissionBody.Type = "user";
                permissionBody.Role = "writer";

                PermissionsResource.CreateRequest createPermRequest = _driveService.Permissions
                    .Create(permissionBody, fileId);
                var permission = new Models.Models.Permission()
                {
                    FileId = fileId,
                    UserId = user.Id
                };
                _context.Permissions.Add(permission);
                _context.SaveChanges();
                createPermRequest.Execute();
            }
            catch (Exception){}
        }
        public Permission SetPermission(Message message)
        {
            var user = _userService.Get(message.From.Username);
            var fileId = user.SpeadsheetId;
            var userToInvite = _userService.GetByGmail(message.Text);
            try
            {
                Permission permissionBody = new Permission();
                permissionBody.EmailAddress = message.Text;
                permissionBody.Type = "user";
                permissionBody.Role = "writer";

                PermissionsResource.CreateRequest createPermRequest = _driveService.Permissions
                    .Create(permissionBody, fileId);

                var permission = new Models.Models.Permission()
                {
                    FileId = fileId,
                    UserId = userToInvite.Id
                };
                userToInvite.SpeadsheetId = fileId;
                _context.Permissions.Add(permission);
                _context.SaveChanges();
                return createPermRequest.Execute();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public bool HasPermission(Message message)
        {
            var user = _userService.Get(message.From.Username);
            var userToInvite = _userService.GetByGmail(message.Text);
            var fileId = user.SpeadsheetId;

            if (userToInvite == null) return false;

            var permissionList = GetPermissionsList(fileId);
            var permission = permissionList.SingleOrDefault(x=>x.UserId == userToInvite.Id);
            if (permission == null) return false;
            return true;

        }
        private List<Models.Models.Permission> GetPermissionsList(string fileId)
        {
            var permissions = _context.Permissions.Where(x => x.FileId == fileId).ToList();
            return permissions;
        }

    }
}
