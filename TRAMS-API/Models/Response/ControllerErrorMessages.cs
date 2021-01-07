namespace API.Models.Response
{
    public class ControllerErrorMessages
    {
        public static string TooManyRequests = "Too many requests, please try again later";
        public static string DownstreamServerError = "Error communicating with downstream server";

        public static string RepositoryErrorLogFormat = "Downstream API failed with status code of {0}. Error: {1}";
    }
}
