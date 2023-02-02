using System;

namespace Dfe.PrepareTransfers.Helpers
{
    public interface IDateTimeProvider
    {
        public DateTime Today();
    }
}