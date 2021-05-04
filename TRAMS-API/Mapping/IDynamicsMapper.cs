namespace API.Mapping
{
    public interface IDynamicsMapper<T, V>
    {
        V Map(T input);
    }
}
