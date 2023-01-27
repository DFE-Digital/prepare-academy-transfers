using Data.Models;

namespace Dfe.PrepareTransfers.Web.Services.Interfaces
{
    public interface IReferenceNumberService
    {
        public string GenerateReferenceNumber(Project project);
    }
}