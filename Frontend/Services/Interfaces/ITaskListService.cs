using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Pages.Projects;

namespace Frontend.Services.Interfaces
{
    public interface ITaskListService
    {
       void BuildTaskListStatuses(string urn, Index indexPage);
    }
}