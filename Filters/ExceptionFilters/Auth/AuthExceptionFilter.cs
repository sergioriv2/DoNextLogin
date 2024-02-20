using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerlessLogin.Dtos;
using System.Net;

namespace ServerlessLogin.Filters.ExceptionFilters.Auth
{
    public class AuthExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            int httpStatusCode = (int)HttpStatusCode.InternalServerError;
            string httpStatusDescription = Status.fromInt(httpStatusCode).Description;
            var errorsResponse = new List<APIResponseError>();

            if (context.Exception is CustomValidationException validationEx)
            {
                switch (validationEx.ExceptionCode)
                {
                    case CustomValidationCodes.InvalidEmail:
                    case CustomValidationCodes.PasswordsDoesntMatch:
                        httpStatusCode = (int)HttpStatusCode.BadRequest;
                        httpStatusDescription = Status.fromInt(httpStatusCode).Description;
                        errorsResponse.Add(
                                new APIResponseError()
                                {
                                    Property = "email",
                                    Constraints = new
                                    {
                                        InvalidCredentials = CustomValidationExceptionsDictionary.Messages[validationEx.ExceptionCode]
                                    }
                                }
                            );
                        break;
                    case CustomValidationCodes.EmailAlreadyOnUse:
                        httpStatusCode = (int)HttpStatusCode.BadRequest;
                        httpStatusDescription = Status.fromInt(httpStatusCode).Description;
                        errorsResponse.Add(
                                new APIResponseError()
                                {
                                    Property = "email",
                                    Constraints = new
                                    {
                                        EmailAlreadyOnUse = CustomValidationExceptionsDictionary.Messages[validationEx.ExceptionCode]
                                    }
                                }
                            );
                        break;
                    case CustomValidationCodes.InvalidEmailCode:
                        httpStatusCode = (int)HttpStatusCode.BadRequest;
                        httpStatusDescription = Status.fromInt(httpStatusCode).Description;
                        errorsResponse.Add(
                                new APIResponseError()
                                {
                                    Property = "email | code",
                                    Constraints = new
                                    {
                                        InvalidCode = CustomValidationExceptionsDictionary.Messages[validationEx.ExceptionCode]
                                    }
                                }
                            );
                        break;
                    case CustomValidationCodes.InvalidRefreshToken:
                        httpStatusCode = (int)HttpStatusCode.Unauthorized;
                        httpStatusDescription = Status.fromInt(httpStatusCode).Description;
                        errorsResponse.Add(
                                new APIResponseError()
                                {
                                    Property = "refreshToken",
                                    Constraints = new
                                    {
                                        InvalidCode = CustomValidationExceptionsDictionary.Messages[validationEx.ExceptionCode]
                                    }
                                }
                            );
                        break;
                    case CustomValidationCodes.ExpiredAccessOrRefreshToken:
                        httpStatusCode = (int)HttpStatusCode.Unauthorized;
                        httpStatusDescription = Status.fromInt(httpStatusCode).Description;
                        errorsResponse.Add(
                                new APIResponseError()
                                {
                                    Property = "accessToken | refreshToken",
                                    Constraints = new
                                    {
                                        InvalidCode = CustomValidationExceptionsDictionary.Messages[validationEx.ExceptionCode]
                                    }
                                }
                            );
                        break;
                    default:
                        break;
                }

            }

            var APIErrorResponse = new APIResponse(
                httpStatusCode,
                httpStatusDescription
            )
            {
                Errors = errorsResponse,
                Payload = new object()
                //{
                //    UnhandledErrorMessage = context.Exception.Message,
                //    UnhandledErrorStackTrace = context.Exception.StackTrace,
                //}
            };

            context.ExceptionHandled = true;
            context.Result = new ObjectResult(APIErrorResponse)
            {
                StatusCode = APIErrorResponse.StatusCode
            };

            Console.WriteLine(context.Exception.ToString());

            return Task.CompletedTask;
        }
    }
}
