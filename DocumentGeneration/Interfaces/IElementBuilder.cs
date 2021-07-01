namespace DocumentGeneration.Interfaces
{
    public interface IElementBuilder<T>
    {
        public T Build();
    }
}