using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OpenBankingApi.Exceptions;
using System.Net;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
            var problemDetails = new ValidationProblemDetails()
            {
                Instance = context.HttpContext.Request.Path,
                Status = typeof(NotFoundException).IsAssignableFrom(context.Exception.GetType()) ? StatusCodes.Status404NotFound : StatusCodes.Status401Unauthorized,
                Detail = "Please refer to the errors property for additional details."
            };
            if (typeof(NotFoundException).IsAssignableFrom(context.Exception.GetType()))
            {
                problemDetails.Status = StatusCodes.Status404NotFound;
            }
            if (typeof(UnauthorizedException).IsAssignableFrom(context.Exception.GetType()))
            {
                problemDetails.Status = StatusCodes.Status401Unauthorized;
            }
            else
            {
                problemDetails.Status = StatusCodes.Status400BadRequest;
            }

            problemDetails.Errors.Add("ValidationErrors", new string[] { context.Exception.Message.ToString() });
            context.Result = new BadRequestObjectResult(problemDetails);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.ExceptionHandled = true;
        }
    }
}
