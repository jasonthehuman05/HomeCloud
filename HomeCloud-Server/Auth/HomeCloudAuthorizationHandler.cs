using Microsoft.AspNetCore.Authorization;

namespace HomeCloud_Server.Auth
{
    public class HomeCloudAuthorizationHandler : AuthorizationHandler<CustomRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequirement requirement)
        {
            //is token in header?
            //No token, fail auth.

            //Token. Is in date?
            //No. Fail auth

            //Token valid.
            //Update Expiry
            //Pass Auth


            //Check. Replace true with the condition to meet
            if (true)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
