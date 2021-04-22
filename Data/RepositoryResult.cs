using System.Net;

namespace Data
{
    public class RepositoryResult<T> : RepositoryResultBase
    {
        public T Result { get; set; }
    }

    public class RepositoryResultBase
    {
        public bool IsValid => Error == null;

        public RepositoryError Error { get; set; }

        public class RepositoryError
        {
            public HttpStatusCode StatusCode { get; set; }

            public string ErrorMessage { get; set; }
        }
    }
}