

using Frontend.Utils;

namespace Frontend.Models
{
    public sealed class Select
    {
        private static string _pageHeader, _backLink, _schoolName;

        public static string BackLink => Typespace.Name(ref _backLink);
        public static string Heading => Typespace.Name(ref _pageHeader);

        public static string SchoolName => Typespace.Name(ref _schoolName);

        public sealed class Common
        {
            private static string _submitButton;
            public static string SubmitButton => Typespace.Name(ref _submitButton);
        }

        public sealed class TaskList
        {
            public sealed class Links
            {
                private static string _legalRequirements;
                public static string LegalRequirements => Typespace.Name(ref _legalRequirements);
            }

            public sealed class LegalRequirements
            {
                private static string _status;
                public static string Status => Typespace.Name(ref _status);
            }
        }

        public sealed class ProjectType
        {
            public sealed class Input
            {
                private static string _conversionOption, _transferOption;

                public static string Conversion => Typespace.Name(ref _conversionOption);
                public static string Transfer => Typespace.Name(ref _transferOption);

            }

        }

        public sealed class Legal
        {
            public sealed class Input
            {
                private static string _yesOption, _noOption, _notApplicableOption;

                public static string Yes => Typespace.Name(ref _yesOption);
                public static string No => Typespace.Name(ref _noOption);
                public static string NotApplicable => Typespace.Name(ref _notApplicableOption);

            }

            public sealed class Summary
            {
                private static string _isComplete, _submitButton;

                public static string IsComplete => Typespace.Name(ref _isComplete);
                public static string SubmitButton => Typespace.Name(ref _submitButton);

                public sealed class GoverningBody
                {
                    private static string _status, _change;

                    public static string Status => Typespace.Name(ref _status);
                    public static string Change => Typespace.Name(ref _change);
                }

                public sealed class Consultation
                {
                    private static string _status, _change;

                    public static string Status => Typespace.Name(ref _status);
                    public static string Change => Typespace.Name(ref _change);
                }

                public sealed class DiocesanConsent
                {
                    private static string _status, _change;

                    public static string Status => Typespace.Name(ref _status);
                    public static string Change => Typespace.Name(ref _change);
                }
            }
        }
    }
}