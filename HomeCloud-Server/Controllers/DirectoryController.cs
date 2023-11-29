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

        /// <summary>
        /// Creates a directory, specifying a name and parent
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="ParentDirectoryID"></param>
        /// <returns></returns>
        [HttpGet("CreateDirectory")]
        public async Task<IActionResult> CreateDirectory(string DirectoryName, uint ParentDirectoryID = 0)
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

        /// <summary>
        /// Gets all directories
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("GetAllDirectories")]
        public async Task<IActionResult> GetAllDirectories()
        {
            List<Models.Directory> d = await _databaseService.GetDirectoriesAsync();
            return Ok(d);
        }

        /// <summary>
        /// Gets all directories inside the specified parent directory
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("GetSubdirectories")]
        public async Task<IActionResult> GetSubdirectories(uint ParentDirectoryID)
        {
            List<Models.Directory> d = await _databaseService.GetSubdirectoriesAsync(ParentDirectoryID);
            return Ok(d);
        }

        [HttpGet("GetFilesInDirectory")]
        public async Task<IActionResult> GetFilesInDirectory(uint DirectoryID)
        {
            List<Models.File> f = await _databaseService.GetAllFilesInDirAsync(DirectoryID);
            return Ok(f);
        }

        /// <summary>
        /// Renames a directory using the provided ID
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("RenameDirectory")]
        public async Task<IActionResult> RenameDirectory(uint DirectoryID, string NewName)
        {
            await _databaseService.RenameDirectoryAsync(DirectoryID, NewName);
            return Ok();
        }


        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("DeleteDirectory")]
        public async Task<IActionResult> DeleteDirectory(uint DirectoryID)
        {
            //Check dir is empty
            //no files contained
            List<Models.Directory> containedDirs = await _databaseService.GetSubdirectoriesAsync(DirectoryID);
            //no directories contained
            List<Models.File> containedFiles = await _databaseService.GetAllFilesInDirAsync(DirectoryID);

            //Check empty
            if(containedDirs.Count == 0 && containedFiles.Count == 0)
            {
                //Delete it
                await _databaseService.DeleteDirectoryAsync(DirectoryID);
                return Ok();
            }
            else
            {
                return Conflict("There were files or folders present in the directory. Please remove these and try again");
            }
        }
    }
}