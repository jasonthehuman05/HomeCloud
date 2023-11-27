using Microsoft.AspNetCore.Mvc;

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

        [HttpGet(Name = "GetFile")]
        public File Get(int FileID)
        {
            return new File
            {
                FileID = FileID,
                FileName = "TestFile.json",
                MIMEType = "applicaiton/json",
                CreatedOn = DateTime.Now,
                PathToData = "C:/users/jason/data/53408275.data"
            };
        }
    }
}