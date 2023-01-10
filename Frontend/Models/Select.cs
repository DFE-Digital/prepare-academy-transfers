using Frontend.Utils;

namespace Frontend.Models
{
    public static class Select
    {
        public static class ProjectType
        {
            public static class Input
            {
                private static string _conversionOption, _transferOption;

                public static string Conversion => Typespace.Name(ref _conversionOption);
                public static string Transfer => Typespace.Name(ref _transferOption);
            }
        }
    }
}