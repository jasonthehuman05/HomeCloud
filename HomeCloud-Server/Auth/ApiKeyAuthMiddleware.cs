namespace HomeCloud_Server.Auth
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Do we have the header?
            if(!context.Request.Headers.TryGetValue("ApiKey", out var extractedApiKey)) //Missing, report back
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing API key");
                return;
            }

            //We have one, check it.
            string ApiKey = "jason";
            if(!ApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Key");
            }

            await _next(context);
        }
    }
}
