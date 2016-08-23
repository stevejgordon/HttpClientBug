using System;
using Microsoft.AspNetCore.Builder;

namespace HttpContextExample.Middleware
{
    public static class RefreshJwtExtensions
    {
        public static IApplicationBuilder UseRefreshJwt(this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<RefreshJwtMiddleware>();
        }
    }
}
