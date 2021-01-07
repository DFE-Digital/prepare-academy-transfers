namespace API.Models.Validation
{
    public class ValidationMessages
    {
        public static string MustNotBeEmpty = "Must not be empty";
        public static string InvalidStatusCode = "Invalid status code";
        public static string DuplicateStatusCode = "Duplicate status code detected";
        public static string WordLengthExceeded = "Must be shorter than {0} words";
        public static string CharLengthExceeded = "Must be shorter than {0} characters";
    }
}
