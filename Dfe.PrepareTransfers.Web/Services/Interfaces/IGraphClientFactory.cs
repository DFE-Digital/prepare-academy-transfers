using Microsoft.Graph;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface IGraphClientFactory
    {
        public GraphServiceClient Create();
    }
}
