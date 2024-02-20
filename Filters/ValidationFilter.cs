using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ServerlessLogin.Filters
{
    public class ValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.Where(values => values.Errors.Count() > 0)
                    .SelectMany(values => values.Errors)
                    .Select(value => value.ErrorMessage)
                    .ToList();

                var response = new
                {
                    statusCode = ((int)HttpStatusCode.BadRequest),
                    message = "Bad Request",
                    payload = new Object(),
                    errors
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = ((int)HttpStatusCode.BadRequest)
                };

                return;
            }

            return;
        }
    }
}
