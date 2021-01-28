using System.Collections.Generic;
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

            return words.Select(w => CapitalizeWord(w)).ToDelimitedString(" ");
        }

        private string CapitalizeWord(string word)
        {
            var brackets = new List<char> { '(', '{', '[', '<' };
            var trimmedWord = word.Trim();

            var stringBuilder = new StringBuilder();

            var handlingOpeningBracket = false;

            if (brackets.Any(b => b == trimmedWord[0]))
            {
                handlingOpeningBracket = true;
            }
            
            stringBuilder.Append(char.ToUpperInvariant(trimmedWord[0]));

            for(var i = 1; i < trimmedWord.Length; i++)
            {
                if(handlingOpeningBracket)
                {
                    if (!brackets.Any(b => b == trimmedWord[i]))
                    {
                        stringBuilder.Append(char.ToUpperInvariant(trimmedWord[i]));
                        handlingOpeningBracket = false;
                    }
                    else
                    {
                        stringBuilder.Append(char.ToLowerInvariant(trimmedWord[i]));
                    }
                }
                else
                {
                    stringBuilder.Append(char.ToLowerInvariant(trimmedWord[i]));
                }
                
            }

            return stringBuilder.ToString();
        }
    }
}