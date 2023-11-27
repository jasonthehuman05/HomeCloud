using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;
using Microsoft.Extensions.Options;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IOptions<ConfigurationService> _configService;
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger, DatabaseService databaseService, IOptions<ConfigurationService> configurationService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _configService = configurationService;
        }

        /// <summary>
        /// Test function. Not to be used for final prod
        /// </summary>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpGet("GetFileTester")]
        public async Task<Models.File> Get(int FileID)
        {
            Models.File testFile = new Models.File
            {
                FileID = FileID,
                FileName = "TestFile.json",
                MIMEType = "applicaiton/json",
                CreatedOnTimestamp = (ulong)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds,
                PathToData = "C:/users/jason/data/53408275.data"
            };
            await _databaseService.AddNewFileAsync(testFile);
            //TODO: MAKE THIS ACTUALLY USEFUL
            return testFile;
        }


        /// <summary>
        /// Takes an upload of files and stores them for use
        /// </summary>
        /// <param name="files">the IFormFile(s) to be stored</param>
        /// <returns></returns>
        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length); //Get total file size in bytes
            List<Models.File> FileList = new List<Models.File>();
            
            foreach(var formFile in files) //Get each file in turn
            {
                if(formFile.Length > 0) //Proceed if it is not empty
                {
                    string newFilePath;
                    while (true)
                    {
                        newFilePath = Utils.GenerateRandomString(16); //Generate a 16 character random file name
                        if (!System.IO.File.Exists(newFilePath)){ break; }
                    }
                    using (var stream = new FileStream(_configService.Value.FileHostingPath + newFilePath, FileMode.Create)) //Create filestream in temp dir, creating a new file
                    {
                        await formFile.CopyToAsync(stream); //copy all data from the file to the new temp file stream
                    }

                    FileList.Add(new Models.File()
                    {
                        FileName = formFile.Name,
                        MIMEType = formFile.ContentType,
                        CreatedOnTimestamp = (ulong)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds, //Current Time
                        PathToData = newFilePath
                    });
                }
            }
            //File was added to the dir
            //Add file to the database
            foreach (Models.File item in FileList)
            {
                await _databaseService.AddNewFileAsync(item); 
            }

            return Ok(new {count = files.Count, size});
        }
    }
}