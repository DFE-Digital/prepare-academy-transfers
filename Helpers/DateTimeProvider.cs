using System;

namespace Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today()
        {
            return DateTime.Today;
        }
    }
}