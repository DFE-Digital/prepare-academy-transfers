namespace Dfe.PrepareTransfers.DocumentGeneration.Elements
{
    public class TextElement
    {
        public TextElement(string value = "")
        {
            Value = value;
        }

        public bool Bold { get; set; }
        public string Colour { get; set; }
        public string FontSize { get; set; }
        public bool Italic { get; set; }
        public bool Underline { get; set; }
        public string Value { get; set; }
    }
}