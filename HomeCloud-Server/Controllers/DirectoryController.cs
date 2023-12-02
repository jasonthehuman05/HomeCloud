using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;
using Microsoft.Extensions.Options;
using HomeCloud_Server.Auth;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [ApiKeyAuthFilter]
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

        private User GetUser()
        {
            string token = Request.Headers["ApiKey"];
            User user = _databaseService.GetUserFromToken(token);
            return user;
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
            User user = GetUser();
            if (PermissionChecker.AllowedToCreate(ParentDirectoryID, user.UserID, _databaseService))
            {
                //Create folder object
                Models.Directory d = new Models.Directory
                {
                    ParentDirectory = ParentDirectoryID,
                    DirectoryName = DirectoryName,
                    CreatedOn = (ulong)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds, //Current Time
                    OwnerID = user.UserID
                };
                //Add folder to the database
                await _databaseService.CreateNewDirectory(d);

                //Return OK
                return Ok($"Folder {DirectoryName} was created.");
            }
            else
            {
                return Unauthorized("You do not have the appropriate permissions to create a new directory here");
            }
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
            User user = GetUser();
            if (PermissionChecker.AllowedToView(user, ParentDirectoryID, _databaseService))
            {
                List<Models.Directory> d = await _databaseService.GetSubdirectoriesAsync(ParentDirectoryID);
                return Ok(d); 
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("GetFilesInDirectory")]
        public async Task<IActionResult> GetFilesInDirectory(uint DirectoryID)
        {
            User user = GetUser();
            if (PermissionChecker.AllowedToView(user, DirectoryID, _databaseService))
            {
                List<Models.File> f = await _databaseService.GetAllFilesInDirAsync(DirectoryID);
                return Ok(f); 
            }
            else { return Unauthorized(); }
        }

        [HttpGet("GetContents")]
        public async Task<IActionResult> GetContents(uint DirectoryID)
        {
            User user = GetUser();
            if (PermissionChecker.AllowedToView(user, DirectoryID, _databaseService))
            {
                Models.DirectoryContents dc = await _databaseService.GetDirectoryContents(DirectoryID);
                return Ok(dc); 
            }
            else { return Unauthorized(); }
        }

        /// <summary>
        /// Renames a directory using the provided ID
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("RenameDirectory")]
        public async Task<IActionResult> RenameDirectory(uint DirectoryID, string NewName)
        {
            User user = GetUser();
            if (PermissionChecker.AllowedToEdit(DirectoryID, user.UserID, _databaseService))
            {
                await _databaseService.RenameDirectoryAsync(DirectoryID, NewName);
                return Ok(); 
            }
            else { return Unauthorized(); }
        }


        /// <summary>
        /// Deletes the specified directory
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete("DeleteDirectory")]
        public async Task<IActionResult> DeleteDirectory(uint DirectoryID)
        {
            User user = GetUser();
            if (PermissionChecker.AllowedToDelete(DirectoryID, user.UserID, _databaseService))
            {
                //Check dir is empty
                //no files contained
                List<Models.Directory> containedDirs = await _databaseService.GetSubdirectoriesAsync(DirectoryID);
                //no directories contained
                List<Models.File> containedFiles = await _databaseService.GetAllFilesInDirAsync(DirectoryID);

                //Check empty
                if (containedDirs.Count == 0 && containedFiles.Count == 0)
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
            else { return Unauthorized(); }
        }
    }
}