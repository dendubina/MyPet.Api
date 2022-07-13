using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyPet.BLL.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MyPet.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            { 
                switch (ex)
                {
                    case ValidationException e:
                        await HandleExceptionAsync(context, e, HttpStatusCode.BadRequest, e.Errors);
                        break;

                    case UnauthorizedAccessException e:
                        await HandleExceptionAsync(context, e, HttpStatusCode.Unauthorized);
                        break;

                    case NotFoundException e:
                        await HandleExceptionAsync(context, e, HttpStatusCode.NotFound);
                        break;

                    case ForbiddenAccessException e:
                        await HandleExceptionAsync(context, e, HttpStatusCode.Forbidden);
                        break;


                    default:
                        _logger.LogCritical($"Unhandled exception caught. Message: {ex.Message}, StackTrace: \r\n {ex.StackTrace}");
                        await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
                        break;
                }
             
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, Dictionary<string, string[]> errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var responseModel = new
            {                
                Status = context.Response.StatusCode,                
                Message = exception.Message,
                Errors = errors,                
            };

            var contractResolver = new DefaultContractResolver // Newtonssoft setting for parameters names in camel case
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string result = JsonConvert.SerializeObject(responseModel, new JsonSerializerSettings { 
             Formatting = Formatting.Indented,
             NullValueHandling = NullValueHandling.Ignore,
             ContractResolver = contractResolver});
            
            await context.Response.WriteAsync(result);
        }
    }
}
