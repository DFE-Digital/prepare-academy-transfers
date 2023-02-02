using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface IGraphUserService
    {
        Task<IEnumerable<User>> GetAllUsers();
    }
}
