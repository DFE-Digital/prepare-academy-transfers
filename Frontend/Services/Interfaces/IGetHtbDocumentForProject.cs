using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services.Responses;

namespace Frontend.Services.Interfaces
{
    public interface IGetHtbDocumentForProject
    {
        public Task<GetProjectTemplateResponse> Execute(string projectUrn);
    }
}