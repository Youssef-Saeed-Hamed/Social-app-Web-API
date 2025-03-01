namespace Graduation_Project.Erorrs
{
    public class ApiResponse
    {
        public ApiResponse(int statusCode, string? messageError = null)
        {
            StatusCode = statusCode;
            MessageError = messageError ?? GetErrorMessage(StatusCode);
        }

        public int StatusCode { get; set; }
        public string? MessageError { get; set; }

        private string? GetErrorMessage(int StatusCode)
            => StatusCode switch
            {
                500 => "Internal Server Error",
                404 => "Not Found",
                401 => "Un Authorized",
                400 => "Bad Request",
                _ => ""
            };

    }
}
