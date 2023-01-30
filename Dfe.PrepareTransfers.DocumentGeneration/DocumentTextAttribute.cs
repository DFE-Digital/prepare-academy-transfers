using System;

namespace Dfe.PrepareTransfers.DocumentGeneration
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DocumentTextAttribute : Attribute
    {
        private readonly string _placeholder;

        public DocumentTextAttribute(string placeholder)
        {
            _placeholder = placeholder;
        }

        public string Placeholder => $"[{_placeholder}]";

        public string Default { get; set; }
        public bool IsRichText { get; set; }
    }
}