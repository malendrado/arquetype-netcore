using Backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Backend.Api.Filters
{
    public class ValidateModelSateAttribute : ActionFilterAttribute
    {
        #region Methods
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            return;
        }
        #endregion
    }
}