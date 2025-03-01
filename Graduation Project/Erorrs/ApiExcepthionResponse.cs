namespace Graduation_Project.Erorrs
{
    public class ApiExcepthionResponse : ApiResponse
    {
        private readonly string? Details;

        public ApiExcepthionResponse(int statusCode, string? messageError = null, string? details = null) : base(statusCode, messageError)
        {
            Details = details;
        }
    }
}
