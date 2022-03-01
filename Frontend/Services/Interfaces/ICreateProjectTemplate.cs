using Frontend.Services.Responses;
using System.Threading.Tasks;

namespace Frontend.Services.Interfaces
{
    public interface ICreateProjectTemplate
    {
        public Task<CreateProjectTemplateResponse> Execute(string projectUrn);
    }
}