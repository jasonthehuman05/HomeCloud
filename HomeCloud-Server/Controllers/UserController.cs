using HomeCloud_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HomeCloud_Server.Services;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

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

        private string GenerateHashedValue(string value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(value);
            byte[] hashed = SHA256.HashData(buffer);
            string hash = Encoding.UTF8.GetString(hashed);
            return hash;
        }

        /// <summary>
        /// Creates a directory, specifying a name and parent
        /// </summary>
        /// <param name="DirectoryName"></param>
        /// <param name="ParentDirectoryID"></param>
        /// <returns></returns>
        [HttpGet("CreateUser")]
        public async Task<IActionResult> CreateDirectory(string UserName, string EmailAddress, string Password)
        {
            //Hash password before it ever gets used
            Password = GenerateHashedValue(Password);
            //Password is now hashed, we can continue

            Models.User user = new User
            {
                UserName = UserName,
                EmailAddress = EmailAddress,
                Password = Password
            };

            _databaseService.CreateNewUser(user);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("LoginUser")]
        public async Task<IActionResult> LoginUser(string EmailAddress, string Password)
        {
            //Hash password so that it is usable
            Password = GenerateHashedValue(Password);

            List<User> users = _databaseService.CheckAccountUsernamePassword(EmailAddress, Password);
            System.Diagnostics.Debug.WriteLine(users.Count);

            if(users.Count == 1)
            {
                //Exactly one account found, valid credentials.
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}