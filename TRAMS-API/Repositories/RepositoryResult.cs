using System.Net;

namespace API.Repositories
{
    public class RepositoryResult<T> : RepositoryResultBase
    {
        public T Result { get; set; }
    }

    public class RepositoryResultBase
    {
        public bool IsValid { get => Error == null; }

        public RepositoryError Error { get; set; }

        public class RepositoryError
        {
            public HttpStatusCode StatusCode { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}
