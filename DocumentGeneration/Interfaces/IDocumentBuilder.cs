using System;
using DocumentGeneration.Interfaces.Parents;

namespace DocumentGeneration.Interfaces
{
    public interface IDocumentBuilder : ITableParent, IParagraphParent
    {
        public void AddHeading(Action<IHeadingBuilder> action);
        public void AddNumberedList(Action<IListBuilder> action);
        public void AddBulletedList(Action<IListBuilder> action);
        public void AddHeader(Action<IHeaderBuilder> action);
        public void AddFooter(Action<IFooterBuilder> action);
        public byte[] Build();
    }
}