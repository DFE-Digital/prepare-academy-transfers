using Dfe.PrepareTransfers.Web.Models.ProjectTemplate;

namespace Dfe.PrepareTransfers.Web.Services.Responses
{
    public class GetProjectTemplateModelResponse
    {
        public GetProjectTemplateModelResponse()
        {
            ResponseError = new ServiceResponseError();
        }
        
        public ProjectTemplateModel ProjectTemplateModel { get; set; }
        public ServiceResponseError ResponseError { get; set; }
        public bool IsValid => string.IsNullOrEmpty(ResponseError.ErrorMessage);
    }
}