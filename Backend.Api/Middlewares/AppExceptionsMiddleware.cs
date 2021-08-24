using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Backend.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Backend.Api.Middlewares
{
    public class AppExceptionsMiddleware
    {
        private readonly RequestDelegate next;

        public AppExceptionsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (AppException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception exceptionObj)
            {
                await HandleExceptionAsync(context, exceptionObj);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, AppException exception)
        {
            string result = null;
            
            context.Response.ContentType = "application/json";
            if (exception is AppException)
            {
                var causas = exception.Data.Values.Cast<object>().ToList();

                result = new ExceptionDetails 
                {
                    Message = exception.Message,
                    StatusCode = (int)exception.StatusCode,
                    Causes = causas.Any() ? causas[0] : null
                }.ToString();
                context.Response.StatusCode = (int)exception.StatusCode;
            }
            else
            {
                var causas = exception.Data.Values.Cast<object>().ToList();
                
                result = new ExceptionDetails 
                { 
                    Message = "Runtime Error",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Causes = causas.Any() ? causas[0] : null
                }.ToString();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            return context.Response.WriteAsync(result);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = new ExceptionDetails 
            { 
                Message = exception.Message,
                StatusCode = (int)HttpStatusCode.InternalServerError 
            }.ToString();
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(result);
        }
    }
}