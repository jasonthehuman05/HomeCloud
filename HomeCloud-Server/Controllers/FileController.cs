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
        public async Task<IActionResult> UploadFile(List<IFormFile> files)
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
                    using (var stream = new FileStream(_configService.Value.FileHostingPath + newFilePath, FileMode.Create)) //Create filestream in dir, creating a new file
                    {
                        await formFile.CopyToAsync(stream); //copy all data from the file to the new temp file stream
                    }
                    FileList.Add(new Models.File()
                    {
                        FileName = Path.GetFileNameWithoutExtension(formFile.FileName),
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

        /// <summary>
        /// Lists all files
        /// </summary>
        /// <returns></returns>
        [HttpGet("ListFiles")]
        public async Task<IActionResult> ListAllFiles()
        {
            //Query database for all files
            List<Models.File> fileList = await _databaseService.GetAllFilesAsync();
            //Return list to the client
            return Ok(fileList);
        }

        /// <summary>
        /// Retrieves file information from the database and loads the file, allowing the client to download it
        /// </summary>
        /// <param name="FileID">The ID to utilise</param>
        /// <returns></returns>
        [HttpGet("RetrieveFile")]
        public async Task<IActionResult> DownloadFile(int FileID)
        {
            //retrieve the file details from the database
            Models.File file = await _databaseService.GetFileAsync(FileID);

            //Does the provided file exist in the directory?
            if (!System.IO.File.Exists(_configService.Value.GetAbsoluteFilePath(file.PathToData)))
            {
                return NotFound(new { path = _configService.Value.GetAbsoluteFilePath(file.PathToData) });
            }
            
            //Open a file stream to get the data from the file
            Stream fileStream = System.IO.File.OpenRead(_configService.Value.GetAbsoluteFilePath(file.PathToData));
            
            //Create the FileResult object to use in the response, applying the correct file name and mime type
            FileResult fileResult = File(fileStream, file.MIMEType, file.FileName);
            return fileResult;
        }

        /// <summary>
        /// Lists all files matching a search criteria
        /// </summary>
        /// <returns></returns>
        [HttpGet("SearchFiles")]
        public async Task<IActionResult> ListAllFiles(string FileName)
        {
            //Query database for all files
            List<Models.File> fileList = await _databaseService.GetAllFilesAsync(FileName);
            //Return list to the client
            return Ok(fileList);
        }

        /// <summary>
        /// Deletes a file from the server, using a provided ID
        /// </summary>
        /// <param name="FileID"></param>
        /// <returns></returns>
        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteFile(int FileID)
        {
            //Get the file details
            Models.File file = await _databaseService.GetFileAsync(FileID);
            
            //Delete the file from dir
            string AbsPath = _configService.Value.GetAbsoluteFilePath(file.PathToData);
            System.IO.File.Delete(AbsPath);

            //Delete the database record
            _databaseService.DeleteFileAsync(FileID);

            return Ok("File was erased.");
        }
    }
}