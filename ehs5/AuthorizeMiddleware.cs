namespace ehs5
{
    public class AuthorizeMiddleware
    {

        private readonly RequestDelegate _requestDelegate;

        public AuthorizeMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Example of adding an Authorization header dynamically
            var token = context.Request.Headers.Authorization;
            context.Request.Headers["Authorization"] = $"Bearer {token}";

            await _requestDelegate(context);
        }
    }
}
