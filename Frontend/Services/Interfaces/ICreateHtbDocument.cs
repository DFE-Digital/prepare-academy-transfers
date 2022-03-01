using Frontend.Services.Responses;
using System.Threading.Tasks;

namespace Frontend.Services.Interfaces
{
    public interface ICreateHtbDocument
    {
        public Task<CreateProjectTemplateResponse> Execute(string projectUrn);
    }
}