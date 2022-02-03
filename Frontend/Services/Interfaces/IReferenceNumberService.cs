using Data.Models;

namespace Frontend.Services.Interfaces
{
    public interface IReferenceNumberService
    {
        public string GenerateReferenceNumber(Project project);
    }
}