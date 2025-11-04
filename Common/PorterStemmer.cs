using System.Text.RegularExpressions;
namespace Common;

public class PorterStemmer
{
    private static readonly PorterStemmer Instance = new();

    public static string[] StemWords(string[] input) => Instance.StemArray(input);

    /// <summary>
    /// Stems an array of words
    /// </summary>
    /// <param name="input">the array of words to be stemmed</param>
    /// <returns>an array containing the stemmed words</returns>
    public string[] StemArray(string[] input)
    {
        string[] result = new string[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            result[i] = StemWord(input[i]);
        }

        return result;
    }

    /// <summary>
    /// Stems a string of words separated by spaces
    /// </summary>
    /// <param name="input">the string of words to be stemmed</param>
    /// <returns>a string containing the stemmed words separated by spaces</returns>
    public string Stem(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        string[] words = input.Split([
            ' ', '\t', '\r', '\n'
        ], StringSplitOptions.RemoveEmptyEntries);
        string[] stemmedWords = StemArray(words);
        return string.Join(" ", stemmedWords);
    }

    // === Core Porter Algorithm ===

    private string StemWord(string word)
    {
        if (word.Length < 3)
            return word.ToLowerInvariant();

        word = word.ToLowerInvariant();

        word = Step1a(word);
        word = Step1b(word);
        word = Step1c(word);
        word = Step2(word);
        word = Step3(word);
        word = Step4(word);
        word = Step5a(word);
        word = Step5b(word);

        return word;
    }

    private static readonly Regex MGr0 = new(@"[aeiouy].*[^aeiou][aeiouy]", RegexOptions.Compiled);
    private static readonly Regex MEq1 = new(@"^[^aeiou]*[aeiouy][^aeiouy]*$", RegexOptions.Compiled);
    private static readonly Regex MGr1 = new(@"^[^aeiou]*([aeiouy][^aeiouy]*){2,}", RegexOptions.Compiled);
    private static readonly Regex VowelInStem = new(@"[aeiouy]", RegexOptions.Compiled);

    private static readonly Regex DoubleConsonant = new(@"([^aeiou])\1$", RegexOptions.Compiled);
    private static readonly Regex Cvc = new(@"[^aeiou][aeiouy][^aeiouywx]$", RegexOptions.Compiled);

    private string Step1a(string word)
    {
        if (word.EndsWith("sses")) return word[..^2];
        if (word.EndsWith("ies")) return word[..^2];
        if (word.EndsWith("ss")) return word;
        if (word.EndsWith("s")) return word[..^1];
        return word;
    }

    private string Step1b(string word)
    {
        bool flag = false;
        if (word.EndsWith("eed"))
        {
            var stem = word[..^3];
            if (MGr0.IsMatch(stem))
                return stem + "ee";
        }
        else if (word.EndsWith("ed"))
        {
            var stem = word[..^2];
            if (VowelInStem.IsMatch(stem))
            {
                word = stem;
                flag = true;
            }
        }
        else if (word.EndsWith("ing"))
        {
            var stem = word[..^3];
            if (VowelInStem.IsMatch(stem))
            {
                word = stem;
                flag = true;
            }
        }

        if (flag)
        {
            if (word.EndsWith("at") || word.EndsWith("bl") || word.EndsWith("iz"))
                word += "e";
            else if (DoubleConsonant.IsMatch(word) && !word.EndsWith("l") && !word.EndsWith("s") && !word.EndsWith("z"))
                word = word[..^1];
            else if (MEq1.IsMatch(word) && Cvc.IsMatch(word))
                word += "e";
        }

        return word;
    }

    private string Step1c(string word)
    {
        if (word.EndsWith("y"))
        {
            var stem = word[..^1];
            if (VowelInStem.IsMatch(stem))
                return stem + "i";
        }
        return word;
    }

    private string Step2(string word)
    {
        string[,] suffixes =
        {
            {
                "ational", "ate"
            },
            {
                "tional", "tion"
            },
            {
                "enci", "ence"
            },
            {
                "anci", "ance"
            },
            {
                "izer", "ize"
            },
            {
                "abli", "able"
            },
            {
                "alli", "al"
            },
            {
                "entli", "ent"
            },
            {
                "eli", "e"
            },
            {
                "ousli", "ous"
            },
            {
                "ization", "ize"
            },
            {
                "ation", "ate"
            },
            {
                "ator", "ate"
            },
            {
                "alism", "al"
            },
            {
                "iveness", "ive"
            },
            {
                "fulness", "ful"
            },
            {
                "ousness", "ous"
            },
            {
                "aliti", "al"
            },
            {
                "iviti", "ive"
            },
            {
                "biliti", "ble"
            }
        };

        for (int i = 0; i < suffixes.GetLength(0); i++)
        {
            string suffix = suffixes[i, 0];
            string replacement = suffixes[i, 1];
            if (word.EndsWith(suffix))
            {
                string stem = word[..^suffix.Length];
                if (MGr0.IsMatch(stem))
                    return stem + replacement;
            }
        }

        return word;
    }

    private string Step3(string word)
    {
        string[,] suffixes =
        {
            {
                "icate", "ic"
            },
            {
                "ative", ""
            },
            {
                "alize", "al"
            },
            {
                "iciti", "ic"
            },
            {
                "ical", "ic"
            },
            {
                "ful", ""
            },
            {
                "ness", ""
            }
        };

        for (int i = 0; i < suffixes.GetLength(0); i++)
        {
            string suffix = suffixes[i, 0];
            string replacement = suffixes[i, 1];
            if (word.EndsWith(suffix))
            {
                string stem = word[..^suffix.Length];
                if (MGr0.IsMatch(stem))
                    return stem + replacement;
            }
        }

        return word;
    }

    private string Step4(string word)
    {
        string[] suffixes =
        {
            "al", "ance", "ence", "er", "ic", "able", "ible", "ant", "ement", "ment", "ent", "ion", "ou", "ism", "ate", "iti", "ous", "ive", "ize"
        };

        foreach (var suffix in suffixes)
        {
            if (word.EndsWith(suffix))
            {
                var stem = word[..^suffix.Length];
                if (suffix == "ion")
                {
                    if (stem.EndsWith("s") || stem.EndsWith("t"))
                        if (MGr1.IsMatch(stem))
                            return stem;
                }
                else if (MGr1.IsMatch(stem))
                    return stem;
            }
        }

        return word;
    }

    private string Step5a(string word)
    {
        if (word.EndsWith("e"))
        {
            var stem = word[..^1];
            if (MGr1.IsMatch(stem) || (MEq1.IsMatch(stem) && !Cvc.IsMatch(stem)))
                return stem;
        }
        return word;
    }

    private string Step5b(string word)
    {
        if (MGr1.IsMatch(word) && DoubleConsonant.IsMatch(word) && word.EndsWith("l"))
            word = word[..^1];
        return word;
    }
}