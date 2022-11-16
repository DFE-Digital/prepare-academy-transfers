using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Services.Interfaces
{
    public interface IGraphUserService
    {
        Task<IEnumerable<User>> GetAllUsers();
    }
}
