using Backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Api.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(GetErrorResponse(context));
                return;
            }
            await next();
        }
        private static ExceptionDetails GetErrorResponse(ActionContext context)
        {
            return new ExceptionDetails
            {
                StatusCode = 400,
                Message = "Los parematers de entrada no son válidos.",
                Causes = (from item in context.ModelState
                          where item.Value.Errors.Any()
                          select new ExceptionCauses { Code = "000", Message = string.Format("{0}: {1}", item.Key, item.Value.Errors[0].ErrorMessage), ProspectoId = "0", Data = null }).ToArray()
            };
        }
    }
}
