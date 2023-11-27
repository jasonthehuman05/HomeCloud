using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetFileTester")]
        public File Get(int FileID)
        {
            //TODO: MAKE THIS ACTUALLY USEFUL
            return new File
            {
                FileID = FileID,
                FileName = "TestFile.json",
                MIMEType = "applicaiton/json",
                CreatedOn = DateTime.Now,
                PathToData = "C:/users/jason/data/53408275.data"
            };
        }

        [HttpPost("UploadFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
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