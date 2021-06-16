namespace Frontend.Services.Responses
{
    public class CreateHtbDocumentResponse
    {
        public CreateHtbDocumentResponse()
        {
            ResponseError = new ServiceResponseError();
        }

        public byte[] Document { get; set; }
        public ServiceResponseError ResponseError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ResponseError.ErrorMessage);
    }
}
