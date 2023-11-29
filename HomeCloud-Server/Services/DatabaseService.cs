﻿using HomeCloud_Server.Models;
using Microsoft.Extensions.Options;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlToolkit;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public async void DeleteFileAsync(int FileID)
        {
            di.NonQueryCommand($"DELETE FROM tblfiles WHERE FileID={FileID};");
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

        #endregion

        #region Directories

        public async Task CreateNewDirectory(Models.Directory d)
        {
            string command = $"INSERT INTO `homecloud`.`tbldirectories` (`ParentDirectoryID`, `DirName`) VALUES({d.ParentDirectory}, '{d.DirectoryName}');";
            Debug.WriteLine(command);
            di.NonQueryCommand(command);
            return;
        }

        public async Task<List<Models.Directory>> GetDirectoriesAsync()
        {
            List<Models.Directory> directories = di.GetData<Models.Directory>("SELECT * FROM tbldirectories;");

            return directories;
        }

        public async Task<List<Models.Directory>> GetSubdirectoriesAsync(uint ParentDirectoryID)
        {
            List<Models.Directory> directories = di.GetData<Models.Directory>($"SELECT * FROM tbldirectories WHERE ParentDirectoryID={ParentDirectoryID};");

            return directories;
        }

        #endregion
    }
}
