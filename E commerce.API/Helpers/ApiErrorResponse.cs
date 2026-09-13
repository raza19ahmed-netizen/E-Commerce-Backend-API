namespace E_commerce.API.Helpers
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public ApiErrorResponse(int statusCode, string message) 
        { 
            Success = false;
            StatusCode = statusCode;
            Message = message;
        
        }
    }
}
