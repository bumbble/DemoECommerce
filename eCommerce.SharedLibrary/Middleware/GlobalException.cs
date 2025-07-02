using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string errorMessage = "An unexpected error occurred. Please try again later.";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // 429 Too Many Requests
                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    errorMessage = "Too many requests. Please try again later.";
                    title = "Rate Limit Exceeded";
                    statusCode = (int)HttpStatusCode.TooManyRequests;
                    await ModifyHeaders(context, errorMessage, title, statusCode);
                }

                // 401 Unauthorized
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    errorMessage = "You are not authorized to access this resource.";
                    title = "Unauthorized Access";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    await ModifyHeaders(context, errorMessage, title, statusCode);
                }

                // 403 Forbidden
                else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    errorMessage = "You do not have permission to access this resource.";
                    title = "Forbidden";
                    statusCode = (int)HttpStatusCode.Forbidden;
                    await ModifyHeaders(context, errorMessage, title, statusCode);
                }

                // 404 Not Found
                else if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    errorMessage = "The requested resource was not found.";
                    title = "Resource Not Found";
                    statusCode = (int)HttpStatusCode.NotFound;
                    await ModifyHeaders(context, errorMessage, title, statusCode);
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                LogException.LogExceptions(ex);

                // Check if exception is timeout
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    errorMessage = "The request timed out. Please try again later.";
                    title = "Request Timeout";
                    statusCode = (int)HttpStatusCode.RequestTimeout;
                }
                else
                {
                    errorMessage = ex.Message;
                    title = "Internal Server Error";
                }

                // If exception is caught
                // If non of exceptions, do default handling
                await ModifyHeaders(context, errorMessage, title, statusCode);
            }
        }

        private static async Task ModifyHeaders(HttpContext context, string errorMessage, string title, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = statusCode,
                Title = title,
                Message = errorMessage
            }, CancellationToken.None);
            return;
        }
    }
}
