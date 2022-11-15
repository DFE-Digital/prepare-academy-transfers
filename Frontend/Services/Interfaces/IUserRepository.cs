using Frontend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();
    }
}
