namespace Data.TRAMS
{
    public interface IMapper<T, V>
    {
        V Map(T input);
    }
}