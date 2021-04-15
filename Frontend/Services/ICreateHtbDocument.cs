using System;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface ICreateHtbDocument
    {
        public Task<byte[]> Execute(Guid projectId);
    }
}