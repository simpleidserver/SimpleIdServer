using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OpenBankingApi.Exceptions;
using System.Net;

namespace SimpleIdServer.OpenBankingApi.Startup.Infrastructure.Filters
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
            if (typeof(NotFoundException).IsAssignableFrom(context.Exception.GetType()))
            {
                var problemDetails = new ValidationProblemDetails()
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status404NotFound,
                    Detail = "Please refer to the errors property for additional details."
                };
                problemDetails.Errors.Add("ValidationErrors", new string[] { context.Exception.Message.ToString() });
                context.Result = new BadRequestObjectResult(problemDetails);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.ExceptionHandled = true;
        }
    }
}
