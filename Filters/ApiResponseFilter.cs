using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerlessLogin.Dtos;

namespace ServerlessLogin.Filters
{
    public class ApiResponseFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult)
            {
                var statusCode = (int)objectResult.StatusCode;
                var apiResponse = new APIResponse()
                {
                    StatusCode = statusCode,
                    Message = Status.fromInt(statusCode).Description,
                    Payload = objectResult.Value

                };
                context.Result = new ObjectResult(apiResponse)
                {
                    StatusCode = statusCode,
                };
            }
            await next();
            return;
        }
    }
}
