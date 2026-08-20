using GoldenCrown.Api.Attributes;
using GoldenCrown.Api.Extentions;
using GoldenCrown.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Api.Middlewares
{
    public class GoldenCrownAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public GoldenCrownAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IApplicationDbContext db)
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
