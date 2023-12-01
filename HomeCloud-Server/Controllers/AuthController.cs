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
    public class AuthController : ControllerBase
    {
        private readonly DatabaseService _databaseService;
        private readonly IOptions<ConfigurationService> _configService;
        private readonly ILogger<FileController> _logger;

        public AuthController(ILogger<FileController> logger, DatabaseService databaseService, IOptions<ConfigurationService> configurationService)
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


        [HttpGet("LoginUser")]
        public async Task<IActionResult> LoginUser(string EmailAddress, string Password)
        {
            //Hash password so that it is usable
            Password = GenerateHashedValue(Password);

            List<User> users = _databaseService.CheckAccountUsernamePassword(EmailAddress, Password);
            System.Diagnostics.Debug.WriteLine(users.Count);

            if (users.Count == 1)
            {
                //Exactly one account found, valid credentials.
                //Generate a token for the account
                string tkn = Utils.GenerateRandomString(32);
                Models.AuthToken token = new Models.AuthToken
                {
                    Token = tkn,
                    UserID = users[0].UserID,
                    ExpiryTimestamp = (ulong)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds + 604800 //7 day duration for a token
                };
                //Add to db
                _databaseService.CreateToken(token);
                return Ok(new {token=token.Token, expiry=token.ExpiryTimestamp});
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}