using System;
using System.Threading.Tasks;

namespace Frontend.Services.Interfaces
{
    public interface ICreateHtbDocument
    {
        public Task<byte[]> Execute(Guid projectId);
    }
}