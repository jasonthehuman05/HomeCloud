using HomeCloud_Server.Models;
using HomeCloud_Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeCloud_Server.Auth
{
    public class ApiKeyAuthFilter : Attribute, IAuthorizationFilter
    {
        private DatabaseService _db;

        public ApiKeyAuthFilter() 
        { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _db = context.HttpContext.RequestServices.GetRequiredService<DatabaseService>();
            //Do we have the header?
            if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey)) //Missing, report back
            {
                context.Result = new UnauthorizedObjectResult("API Key Missing");
                return;
            }
            //We had a key, it is now in the extractedApiKey variable.
            
            //Get all tokens
            List<AuthToken> tokens = _db.GetTokens();

            //Try and get the token from the list
            AuthToken token = tokens.FirstOrDefault(token => token.Token == extractedApiKey, null);

            if (token == null) //No matching tokens
            {
                context.Result = new UnauthorizedObjectResult("API Key Invalid");
            }
            else
            {
                //We found the token. Allow the application to continue, leave the token expiry to update in the background
                _db.UpdateTokenExpiry(token);
            }
        }
    }
}
