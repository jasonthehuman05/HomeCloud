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
    //DO NOT REQUIRE AUTH ISTG
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
                return Ok(new { token = token.Token, expiry = token.ExpiryTimestamp });
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Check the provided token to see if it is valid and in date
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("VerifyToken")]
        public async Task<IActionResult> VerifyToken(string token)
        {
            //Get all tokens
            List<AuthToken> tokens = _databaseService.GetTokens();

            //Try and get the token from the list
            AuthToken tkn = tokens.FirstOrDefault(_t => _t.Token == token, null);

            if (tkn == null) //No matching tokens
            {
                return BadRequest("No token was provided");
            }
            else if(tkn.ExpiryTimestamp < DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds)
            {
                return BadRequest("Token has expired");
            }
            else
            {
                //We found the token. Allow the application to continue, leave the token expiry to update in the background
                AuthToken t = _databaseService.UpdateTokenExpiry(tkn);
                return Ok(token);
            }
        }
    }
}