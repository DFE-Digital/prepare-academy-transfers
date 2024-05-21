namespace Dfe.PrepareConversions.Extensions;

using Dfe.PrepareTransfers.Web.ExtensionMethods;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System;

public static class StringExtensions
{
    public static string SplitPascalCase<T>(this T source)
    {
        if (source != null)
        {
            return Regex.Replace(source.ToString() ?? string.Empty, "[A-Z]", " $0", RegexOptions.None, TimeSpan.FromSeconds(1.0)).Trim();
        }

        return string.Empty;
    }

    public static string ToSentenceCase(this string input, bool ignoreAcronyms = true)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        string[] array = input.Split(' ');
        bool flag = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (!ignoreAcronyms || !IsAcronym(array[i]))
            {
                array[i] = array[i].ToLowerInvariant();
                if (!flag)
                {
                    int num = i;
                    string text = char.ToUpperInvariant(array[i][0]).ToString();
                    string text2 = array[i];
                    array[num] = text + text2.Substring(1, text2.Length - 1);
                    flag = true;
                }
            }
        }

        return string.Join(' ', array);
    }

    public static bool ToBool(this string str)
    {
        string text = str.ToLower();
        if (!(text == "yes"))
        {
            if (text == "no")
            {
                return false;
            }

            throw new ArgumentException("The string must be either 'Yes' or 'No'.");
        }

        return true;
    }

    public static bool IsAcronym(string word)
    {
        if (!string.IsNullOrEmpty(word) && word.Length >= 2 && char.IsUpper(word[0]))
        {
            return char.IsUpper(word[word.Length - 1]);
        }

        return false;
    }

    public static bool IsAllCaps(string word)
    {
        if (!string.IsNullOrEmpty(word))
        {
            return word.All(char.IsUpper);
        }

        return false;
    }

    public static string ToTitleCase(this string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    public static bool IsEmpty(this string input)
    {
        return string.IsNullOrWhiteSpace(input);
    }

    public static bool IsPresent(this string input)
    {
        return !input.IsEmpty();
    }

    public static string SquishToLower(this string input)
    {
        return input.Replace(" ", "").ToLowerInvariant();
    }

    public static string ToFirstUpper(this string input)
    {
        string text = input.ToLower();
        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
        defaultInterpolatedStringHandler.AppendFormatted(char.ToUpper(text[0]));
        string text2 = text;
        defaultInterpolatedStringHandler.AppendFormatted(text2.Substring(1, text2.Length - 1));
        return defaultInterpolatedStringHandler.ToStringAndClear();
    }

    public static string ToHyphenated(this string str)
    {
        return new Regex("\\s+", RegexOptions.None, TimeSpan.FromSeconds(1.0)).Replace(str, "-");
    }

    public static string RemoveNonAlphanumericOrWhiteSpace(this string str)
    {
        return new Regex("[^\\w\\s-]", RegexOptions.None, TimeSpan.FromSeconds(1.0)).Replace(str, string.Empty);
    }
}
