using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request is coming from the API Gateway
            if (context.Request.Headers.ContainsKey("Api-Gateway"))
            {
                // Proceed with the request
                await next(context);
            }
            else
            {
                // If not from API Gateway, return 503 Service Unavailable
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Access denied. This endpoint is only accessible through the API Gateway.");
                return;
            }
        }
    }
}
