using Data.Models;

namespace Frontend.Services.Interfaces
{
    public interface IReferenceNumberService
    {
        public string GetReferenceNumber(Project project);
    }
}