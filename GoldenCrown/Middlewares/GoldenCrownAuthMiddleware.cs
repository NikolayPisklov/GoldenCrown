using GoldenCrown.Attributes;
using GoldenCrown.Database;
using GoldenCrown.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace GoldenCrown.Middlewares
{
    public class GoldenCrownAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public GoldenCrownAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, GoldenCrownDbContext db)
        {
            var endpoint = context.GetEndpoint();
            bool hasAttribute = endpoint?.Metadata.GetMetadata<GoldenCrownAuth>() != null;
            if (hasAttribute) 
            {
                var token = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var session = await db.Sessions.FirstOrDefaultAsync(x => x.Token == token.ToString());
                if (session is null || session.ExpiresAt < DateTime.UtcNow) 
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                context.SetUserId(session.UserId);
            }
            await _next(context);
        }
    }
}
