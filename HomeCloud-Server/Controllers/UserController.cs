using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Cryptography;

namespace HomeCloud_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IOptions<ConfigurationService> _configService;
        private readonly ILogger<FileController> _logger;

        public UserController(ILogger<FileController> logger, DatabaseService databaseService, IOptions<ConfigurationService> configurationService)
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
        [HttpGet("CreateUser")]
        public async Task<IActionResult> CreateDirectory(string EmailAddress, string Password)
        {
            //Hash password before it ever gets used
            byte[] buffer = Encoding.UTF8.GetBytes(Password);
            byte[] hashed = SHA256.HashData(buffer);
            Password = Encoding.UTF8.GetString(hashed);
            //Password is now hashed, we can continue

            Models.User user = new User
            {
                EmailAddress = EmailAddress,
                Password = Password
            };

            _databaseService.CreateNewUser(user);

            return Ok();
        }
    }
}