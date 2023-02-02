namespace Dfe.PrepareTransfers.Web.Services.Responses
{
    public class ServiceResponseError
    {
        public ErrorCode ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum ErrorCode
    {
        Default = 0,
        NotFound,
        ApiError
    }
}