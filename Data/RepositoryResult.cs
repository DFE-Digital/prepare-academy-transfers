namespace Data
{
    public class RepositoryResult<T>
    {
        public T Result { get; set; }
        public int TotalRecords { get; set; }
    }
}