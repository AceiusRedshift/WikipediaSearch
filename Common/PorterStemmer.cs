namespace Indexer;

// C# port of provided scala code

/// <summary>
/// Provides a contract for Porter Stemmer (and other Stemmers).
/// Serves as an API.
/// </summary>
public interface IStemmer
{
    /// <summary>
    /// Stems an array of words
    /// </summary>
    /// <param name="input">the array of words to be stemmed</param>
    /// <returns>an array containing the stemmed words</returns>
    string[] StemArray(string[] input);

    /// <summary>
    /// Stems a string of words separated by spaces
    /// </summary>
    /// <param name="input">the string of words to be stemmed</param>
    /// <returns>a string containing the stemmed words separated by spaces</returns>
    string Stem(string input);
}

/// <summary>
/// Porter stemmer in C# (converted from Scala).
/// Original paper:
/// Porter, 1980, An algorithm for suffix stripping, Program, Vol. 14, no. 3, pp 130–137
/// See also http://www.tartarus.org/~martin/PorterStemmer
/// </summary>
public class PorterStemmer : IStemmer
{
    /// <summary>
    /// Stems one word using a given stemmer instance.
    /// </summary>
    public static string StemOneWord(string input, PorterStemmerImpl stemmer)
    {
        stemmer.Add(input.Trim());

        if (stemmer.Word.Length > 2)
        {
            stemmer.Step1();
            stemmer.Step2();
            stemmer.Step3();
            stemmer.Step4();
            stemmer.Step5a();
            stemmer.Step5b();
        }

        return stemmer.Word;
    }

    public string Stem(string input)
    {
        var stemmer = new PorterStemmerImpl();
        string[] wordList = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var word in wordList)
        {
            stemmer.Add(word.Trim());

            if (stemmer.Word.Length > 2)
            {
                stemmer.Step1();
                stemmer.Step2();
                stemmer.Step3();
                stemmer.Step4();
                stemmer.Step5a();
            }

            result.Add(stemmer.Word);
        }

        return string.Join(" ", result);
    }

    public string[] StemArray(string[] input)
    {
        var s = new PorterStemmerImpl();
        string[] result = new string[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            result[i] = StemOneWord(input[i], s);
        }
        return result;
    }
}

public class PorterStemmerImpl
{
    public string Word { get; set; } = "";

    private bool Cons(int i)
    {
        char ch = Word[i];
        string vowels = "aeiou";

        if (vowels.Contains(ch))
            return false;

        if (ch == 'y')
        {
            if (i == 0) return true;
            return !Cons(i - 1);
        }

        return true;
    }

    public void Add(char ch)
    {
        Word += ch;
    }

    public void Add(string newWord)
    {
        Word = newWord;
    }

    public int CalcM(string s)
    {
        int count = 0;
        bool currentConst = false;

        for (int c = 0; c < s.Length; c++)
        {
            if (Cons(c))
            {
                if (!currentConst && c != 0)
                    count++;
                currentConst = true;
            }
            else
            {
                currentConst = false;
            }
        }

        return count;
    }

    public bool VowelInStem(string s)
    {
        for (int i = 0; i < Word.Length - s.Length; i++)
        {
            if (!Cons(i))
                return true;
        }
        return false;
    }

    public bool DoubleC()
    {
        int i = Word.Length - 1;
        if (i < 1) return false;
        if (Word[i] != Word[i - 1]) return false;
        return Cons(i);
    }

    public bool Cvc(string s)
    {
        int i = Word.Length - 1 - s.Length;
        if (i < 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2))
            return false;

        char ch = Word[i];
        string vals = "wxy";
        return !vals.Contains(ch);
    }

    public bool Replacer(string orig, string replace, Func<int, bool> checker)
    {
        int l = Word.Length;
        int origLength = orig.Length;
        bool res = false;

        if (Word.EndsWith(orig))
        {
            string n = Word.Substring(0, l - origLength);
            int m = CalcM(n);

            if (checker(m))
            {
                Word = n + replace;
            }
            res = true;
        }

        return res;
    }

    public bool ProcessSubList(List<(string, string)> list, Func<int, bool> checker)
    {
        foreach (var v in list)
        {
            if (Replacer(v.Item1, v.Item2, checker))
                return true;
        }
        return false;
    }

    public void Step1()
    {
        int m = CalcM(Word);

        var vals = new List<(string, string)>
        {
            ("sses", "ss"), ("ies", "i"), ("ss", "ss"), ("s", "")
        };
        ProcessSubList(vals, _ => true);

        if (!Replacer("eed", "ee", x => x > 0))
        {
            if ((VowelInStem("ed") && Replacer("ed", "", _ => true)) ||
                (VowelInStem("ing") && Replacer("ing", "", _ => true)))
            {
                vals = new List<(string, string)> { ("at", "ate"), ("bl", "ble"), ("iz", "ize") };

                if (!ProcessSubList(vals, _ => true))
                {
                    m = CalcM(Word);
                    char last = Word[Word.Length - 1];

                    if (DoubleC() && !"lsz".Contains(last))
                    {
                        Word = Word.Substring(0, Word.Length - 1);
                    }
                    else if (m == 1 && Cvc(""))
                    {
                        Word += "e";
                    }
                }
            }
        }

        if (VowelInStem("y"))
            Replacer("y", "i", _ => true);
    }

    public void Step2()
    {
        var vals = new List<(string, string)>
        {
            ("ational", "ate"), ("tional", "tion"), ("enci", "ence"), ("anci", "ance"),
            ("izer", "ize"), ("bli", "ble"), ("alli", "al"), ("entli", "ent"), ("eli", "e"),
            ("ousli", "ous"), ("ization", "ize"), ("ation", "ate"), ("ator", "ate"),
            ("alism", "al"), ("iveness", "ive"), ("fulness", "ful"), ("ousness", "ous"),
            ("aliti", "al"), ("iviti", "ive"), ("biliti", "ble"), ("logi", "log")
        };
        ProcessSubList(vals, x => x > 0);
    }

    public void Step3()
    {
        var vals = new List<(string, string)>
        {
            ("icate", "ic"), ("ative", ""), ("alize", "al"),
            ("iciti", "ic"), ("ical", "ic"), ("ful", ""), ("ness", "")
        };
        ProcessSubList(vals, x => x > 0);
    }

    public void Step4()
    {
        var vals = new List<(string, string)>
        {
            ("al", ""), ("ance", ""), ("ence", ""), ("er", ""), ("ic", ""),
            ("able", ""), ("ible", ""), ("ant", ""), ("ement", ""), ("ment", ""), ("ent", "")
        };

        bool res = ProcessSubList(vals, x => x > 1);

        if (!res && Word.Length > 4)
        {
            if (Word[Word.Length - 4] == 's' || Word[Word.Length - 4] == 't')
            {
                res = Replacer("ion", "", x => x > 1);
            }
        }

        if (!res)
        {
            vals = new List<(string, string)>
            {
                ("ou", ""), ("ism", ""), ("ate", ""), ("iti", ""),
                ("ous", ""), ("ive", ""), ("ize", "")
            };
            ProcessSubList(vals, x => x > 1);
        }
    }

    public void Step5a()
    {
        Replacer("e", "", x => x > 1);

        if (!Cvc("e"))
        {
            Replacer("e", "", x => x == 1);
        }
    }

    public void Step5b()
    {
        int m = CalcM(Word);
        if (m > 1 && DoubleC() && Word.EndsWith("l"))
        {
            Word = Word.Substring(0, Word.Length - 1);
        }
    }
}