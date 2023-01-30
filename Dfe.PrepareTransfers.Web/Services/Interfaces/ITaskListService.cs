using System.Threading.Tasks;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Pages.Projects;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface ITaskListService
    {
       void BuildTaskListStatuses(Index indexPage);
    }
}