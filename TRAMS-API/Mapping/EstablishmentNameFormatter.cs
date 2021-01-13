using System.Linq;
using System.Text;

namespace API.Mapping
{
    public class EstablishmentNameFormatter : IEstablishmentNameFormatter
    {
        public string Format(string input)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var words = input.Split(' ').Where(w => !string.IsNullOrEmpty(w) && !string.IsNullOrWhiteSpace(w));

            return string.Join(' ', words.Select(w => CapitalizeWord(w)));
        }

        private string CapitalizeWord(string word)
        {
            var trimmedWord = word.Trim();

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(char.ToUpperInvariant(trimmedWord[0]));

            for(var i = 1; i < trimmedWord.Length; i++)
            {
                stringBuilder.Append(char.ToLowerInvariant(trimmedWord[i]));
            }

            return stringBuilder.ToString();
        }
    }
}