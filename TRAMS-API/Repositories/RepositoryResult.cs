using System.Net;

namespace API.Repositories
{
    public class RepositoryResult<T>
    {
        public T Result { get; set; }

        public RepositoryError Error { get;set;}

        public class RepositoryError
        {
            public HttpStatusCode StatusCode { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}
