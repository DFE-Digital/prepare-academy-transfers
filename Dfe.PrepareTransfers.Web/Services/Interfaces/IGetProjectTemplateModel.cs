using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Services.Responses;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface IGetProjectTemplateModel
    {
        public Task<GetProjectTemplateModelResponse> Execute(string projectUrn);
    }
}