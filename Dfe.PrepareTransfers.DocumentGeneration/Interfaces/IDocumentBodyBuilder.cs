using System;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces.Parents;

namespace Dfe.PrepareTransfers.DocumentGeneration.Interfaces
{
    public interface IDocumentBodyBuilder : ITableParent, IParagraphParent
    {
        public void AddHeading(Action<IHeadingBuilder> action);
        public void AddNumberedList(Action<IListBuilder> action);
        public void AddBulletedList(Action<IListBuilder> action);
    }
}