namespace ServerlessLogin.Dtos
{
   public class APIResponseError
    {
        public string Property { get; set; }
        public object Constraints { get; set; }
    }

    public class APIResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Payload { get; set; }
        public List<APIResponseError> Errors { get; set; } = new List<APIResponseError>();

        public APIResponse()
        {
            
        }

        public APIResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

    }

    public class APIResponseDto<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Payload { get; set; }
        public List<APIResponseError> Errors { get; set; } = new List<APIResponseError>();
    }
}
