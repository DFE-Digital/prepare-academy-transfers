using Frontend.Models;

namespace Frontend.Services.Responses
{
    public class GetHtbDocumentForProjectResponse
    {
        public GetHtbDocumentForProjectResponse()
        {
            ResponseError = new ServiceResponseError();
        }
        
        public HtbDocument HtbDocument { get; set; }
        public ServiceResponseError ResponseError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ResponseError.ErrorMessage);
    }
}