using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeCloud_Server.Auth
{
    public class ApiKeyAuthFilter : Attribute, IAuthorizationFilter
    {
        public ApiKeyAuthFilter() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Do we have the header?
            if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey)) //Missing, report back
            {
                context.Result = new UnauthorizedObjectResult("API Key Missing");
                return;
            }

            //We have one, check it.
            string ApiKey = "jason";
            if (!ApiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key Invalid");
            }
        }
    }
}
