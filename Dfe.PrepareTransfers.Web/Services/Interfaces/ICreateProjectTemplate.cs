using Dfe.PrepareTransfers.Web.Services.Responses;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface ICreateProjectTemplate
    {
        public Task<CreateProjectTemplateResponse> Execute(string projectUrn);
    }
}