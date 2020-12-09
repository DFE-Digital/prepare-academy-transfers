namespace API.Mapping
{
    public interface IMapper<T, V>
    {
        V Map(T input);
    }
}
