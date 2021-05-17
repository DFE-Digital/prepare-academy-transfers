using System;

namespace Frontend.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Today()
        {
            return DateTime.Today;
        }
    }
}