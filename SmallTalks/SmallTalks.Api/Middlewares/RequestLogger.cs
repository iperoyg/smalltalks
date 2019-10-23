using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallTalks.Api.Middlewares
{
    public class RequestLogger
    {
        readonly RequestDelegate _next;

        public RequestLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //context.TraceIdentifier
            context.Request.Headers.TryGetValue("Authorization", out StringValues authentication);
            var requestId = context.TraceIdentifier;

            using (LogContext.PushProperty("Authorization", authentication, destructureObjects: true))
            {
                await _next.Invoke(context);
            }
        }
    }
}
