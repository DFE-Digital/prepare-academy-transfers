namespace Dfe.PrepareTransfers.Data.TRAMS
{
    public interface IMapper<in T, out V>
    {
        V Map(T input);
    }
}