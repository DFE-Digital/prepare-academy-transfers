using System;

namespace Dfe.PrepareTransfers.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today()
        {
            return DateTime.Today;
        }
    }
}