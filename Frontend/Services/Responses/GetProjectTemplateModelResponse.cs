using Frontend.Models.ProjectTemplate;

namespace Frontend.Services.Responses
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