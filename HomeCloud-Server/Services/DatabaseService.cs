using HomeCloud_Server.Models;
using HomeCloud_Server.Services.MySqlInteractionToolkit;
using Microsoft.Extensions.Options;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlToolkit;
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
            // MySqlToolkit.InsertData<Models.File>("tblFiles", file);
            di.InsertData<Models.File>("tblfiles", file);
            return;
        }

        #endregion
    }
}
