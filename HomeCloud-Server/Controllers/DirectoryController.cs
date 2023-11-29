using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;
using Microsoft.Extensions.Options;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DirectoryController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IOptions<ConfigurationService> _configService;
        private readonly ILogger<FileController> _logger;

        public DirectoryController(ILogger<FileController> logger, DatabaseService databaseService, IOptions<ConfigurationService> configurationService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _configService = configurationService;
        }

        [HttpGet("CreateDirectory")]
        public async Task<IActionResult> CreateDirectory(string DirectoryName, uint ParentDirectoryID)
        {
            //Create folder object
            Models.Directory d = new Models.Directory
            {
                ParentDirectory = ParentDirectoryID,
                DirectoryName = DirectoryName
            };
            //Add folder to the database
            await _databaseService.CreateNewDirectory(d);

            //Return OK
            return Ok($"Folder {DirectoryName} was created.");
        }
    }
}