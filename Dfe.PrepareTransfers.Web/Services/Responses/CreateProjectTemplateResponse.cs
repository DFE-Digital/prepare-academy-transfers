namespace Dfe.PrepareTransfers.Web.Services.Responses
{
    public class CreateProjectTemplateResponse
    {
        public CreateProjectTemplateResponse()
        {
            ResponseError = new ServiceResponseError();
        }

        public byte[] Document { get; set; }
        public ServiceResponseError ResponseError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ResponseError.ErrorMessage);
    }
}
