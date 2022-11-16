using Microsoft.Graph;

namespace Frontend.Services.Interfaces
{
    public interface IGraphClientFactory
    {
        public GraphServiceClient Create();
    }
}
