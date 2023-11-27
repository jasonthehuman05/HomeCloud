using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

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

        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            // GET THE FILE AND SAVE IT

            long size = files.Sum(f => f.Length); //Get total file size in bytes
            List<string> filePaths = new List<string>();
            foreach(var formFile in files) //Get each file in turn
            {
                if(formFile.Length > 0) //Proceed if it is not empty
                {
                    var filePath = Path.GetTempFileName(); //Get temp path
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(filePath, FileMode.Create)) //Create filestream in temp dir, creating a new file
                    {
                        await formFile.CopyToAsync(stream); //copy all data from the file to the new temp file stream
                    }
                }
            }

            return Ok(new {count = files.Count, size, filePaths});
        }
    }
}