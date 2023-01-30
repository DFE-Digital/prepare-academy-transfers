namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface IElementBuilder<out T>
    {
        public T Build();
    }
}