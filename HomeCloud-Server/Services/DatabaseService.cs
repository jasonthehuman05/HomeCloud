using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders.Composite;
using Microsoft.Extensions.Options;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlToolkit;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HomeCloud_Server.Services
{
    public class DatabaseService
    {
        DatabaseInteractor di;

        public DatabaseService(IOptions<DatabaseSettings> settings) 
        {
            //Create conneciton string
            string CONN_STRING = $"server={settings.Value.ipAddress};userid={settings.Value.username};password={settings.Value.password};database={settings.Value.databaseName};port={settings.Value.port}";
            Debug.WriteLine(CONN_STRING);


            //Open Connection
            di = new DatabaseInteractor(CONN_STRING);
            Debug.WriteLine("Conn Open");
        }

        #region Files

        public async Task AddNewFileAsync(Models.File file)
        {
            //Insert into the db
            di.InsertData<Models.File>("tblfiles", file);
            return;
        }

        public async Task<Models.File> GetFileAsync(int FileID)
        {
            List<Models.File> retrievedFiles = di.GetData<Models.File>($"SELECT * FROM tblfiles WHERE FileID={FileID}");
            Debug.WriteLine("Retrieved " + retrievedFiles.Count + " files");
            return retrievedFiles[0];
        }


        public async Task<List<Models.File>> GetAllFilesAsync()
        {
            List<Models.File> retrievedFiles = di.GetData<Models.File>($"SELECT * FROM tblfiles;");
            return retrievedFiles;
        }

        public async Task<List<Models.File>> GetAllFilesAsync(string FileName)
        {
            List<Models.File> retrievedFiles = di.GetData<Models.File>($"SELECT * FROM tblfiles WHERE FileName LIKE \"%{FileName}%\";");
            return retrievedFiles;
        }

        public async Task<List<Models.File>> GetAllFilesInDirAsync(uint DirectoryID)
        {
            List<Models.File> retrievedFiles = di.GetData<Models.File>($"SELECT * FROM tblfiles WHERE ParentDirectoryID={DirectoryID};");
            return retrievedFiles;
        }

        public async Task MoveFileAsync(int FileID, uint DirectoryID)
        {
            di.NonQueryCommand($"UPDATE tblfiles SET ParentDirectoryID={DirectoryID} WHERE FileID={FileID};");
            return;
        }

        public async void DeleteFileAsync(int FileID)
        {
            di.NonQueryCommand($"DELETE FROM tblfiles WHERE FileID={FileID};");
        }
        #endregion

        #region Directories

        public async Task CreateNewDirectory(Models.Directory d)
        {
            di.InsertData<Models.Directory>("tbldirectories", d);
            return;
        }

        public async Task<List<Models.Directory>> GetDirectoriesAsync()
        {
            List<Models.Directory> directories = di.GetData<Models.Directory>("SELECT * FROM tbldirectories;");

            return directories;
        }

        public Models.Directory GetDirectory(ulong dirID)
        {
            Models.Directory directory = di.GetData<Models.Directory>($"SELECT * FROM tbldirectories WHERE DirectoryID={dirID};")[0];

            return directory;
        }

        public async Task<List<Models.Directory>> GetSubdirectoriesAsync(uint ParentDirectoryID)
        {
            List<Models.Directory> directories = di.GetData<Models.Directory>($"SELECT * FROM tbldirectories WHERE ParentDirectoryID={ParentDirectoryID};");

            return directories;
        }

        public async Task RenameDirectoryAsync(uint DirectoryID, string NewName)
        {
            di.NonQueryCommand($"UPDATE tbldirectories SET DirName='{NewName}' WHERE DirectoryID={DirectoryID};");
            return;
        }

        public async Task DeleteDirectoryAsync(uint DirectoryID)
        {
            di.NonQueryCommand($"DELETE FROM tbldirectories WHERE DirectoryID={DirectoryID};");
        }

        public async Task<Models.DirectoryContents> GetDirectoryContents(uint DirectoryID)
        {
            //Get Directories
            List<Models.Directory> directories = await GetSubdirectoriesAsync(DirectoryID);
            //Get Files
            List<Models.File> files = await GetAllFilesInDirAsync(DirectoryID);
            //Combine
            Models.DirectoryContents dc = new DirectoryContents
            {
                Directories = directories,
                Files = files
            };
            //Return
            return dc;
        }

        #endregion

        #region Users

        public async Task CreateNewUser(Models.User user)
        {
            di.InsertData<Models.User>("tblusers", user);
        }

        internal List<User> CheckAccountUsernamePassword(string emailAddress, string password)
        {
            //Build Query
            string query = $"SELECT * FROM `tblusers` WHERE `EmailAddress`=\"{emailAddress}\" AND `Password`=\"{password}\";";
            List<User> accounts = di.GetData<Models.User>(query);

            return accounts;
        }

        internal User GetUser(ulong UserID)
        {
            //Build Query
            string query = $"SELECT * FROM `tblusers` WHERE `UserID`={UserID};";
            User user = di.GetData<Models.User>(query)[0];

            return user;
        }

        #endregion

        #region auth

        internal void CreateToken(AuthToken token)
        {
            di.InsertData<AuthToken>("tblauthtokens", token);
        }

        internal List<AuthToken> GetTokens()
        {
            string query = "SELECT * FROM tblauthtokens;";
            List<AuthToken> tokens = di.GetData<AuthToken>(query);
            return tokens;
        }

        internal User GetUserFromToken(string token)
        {
            string query = $"SELECT * FROM tblauthtokens WHERE Token=\"{token}\";";
            AuthToken TokenObject = di.GetData<AuthToken>(query)[0];
            string getUserQuery = $"SELECT * FROM tblusers WHERE UserID=\"{TokenObject.UserID}\";";
            User user = di.GetData<User>(getUserQuery)[0];
            return user;
        }

        internal AuthToken UpdateTokenExpiry(AuthToken token)
        {
            ulong time = (ulong)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds + 604800; //7 day token duration
            di.NonQueryCommand($"UPDATE tblauthtokens SET ExpiryTimestamp='{time}' WHERE Token=\"{token.Token}\";");
            token.ExpiryTimestamp = time;
            return token;
        }

        internal void DeleteToken(string token)
        {
            di.NonQueryCommand($"DELETE FROM tblauthtokens WHERE Token=\"{token}\";");
        }

        #endregion

        #region Permissions

        public List<Models.DirectoryAccessRights> GetUserDirectoryPermissions(ulong UserID, int DirectoryID)
        {
            List<Models.DirectoryAccessRights> rights = di.GetData<Models.DirectoryAccessRights>($"SELECT * FROM tbldirectoryaccessrights WHERE DirectoryID={DirectoryID} AND UserID={UserID};");
            return rights;
        }

        internal void UpdateExistingDirectoryPermission(ulong directoryAccessRightsID, bool cC, bool cV, bool cE, bool cD)
        {
            di.NonQueryCommand($"UPDATE tbldirectoryaccessrights SET CanCreate={cC}, CanView={cV}, CanEdit={cE}, CanDelete={cD} WHERE DirectoryAccessRightsID={directoryAccessRightsID};");
            return;
        }

        internal void CreateDirectoryPermission(ulong directoryID, int userID, bool cC, bool cV, bool cE, bool cD)
        {
            DirectoryAccessRights dir = new DirectoryAccessRights()
            {
                DirectoryID = directoryID,
                UserID = (ulong)userID,
                CanCreate = cC,
                CanView = cV,
                CanEdit = cE,
                CanDelete = cV
            };
            di.InsertData<DirectoryAccessRights>("tbldirectoryaccessrights", dir);
        }

        #endregion
    }
}
