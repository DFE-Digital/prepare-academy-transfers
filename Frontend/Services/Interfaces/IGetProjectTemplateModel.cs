using System.Threading.Tasks;
using Frontend.Services.Responses;

namespace Frontend.Services.Interfaces
{
    public interface IGetProjectTemplateModel
    {
        public Task<GetProjectTemplateModelResponse> Execute(string projectUrn);
    }
}