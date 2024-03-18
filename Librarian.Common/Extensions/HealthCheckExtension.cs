using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Extensions
{
    public static class HealthCheckExtension
    {
        public static void UseHealthCheck(this IEndpointRouteBuilder app, string path = "/health")
        {
            app.MapGet(path, ctx =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                return ctx.Response.WriteAsync("OK");
            });
        }
    }
}
