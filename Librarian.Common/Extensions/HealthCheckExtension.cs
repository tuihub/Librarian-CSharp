using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Librarian.Common.Extensions;

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