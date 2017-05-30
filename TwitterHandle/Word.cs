using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterHandle
{
    public class Word
    {
        public static int CountOccurrencesIgnoreCase(string substring, string text)
        {
            int count = 0;
            int index = -1;
            while (true)
            {
                index = text.IndexOf(substring, index + 1,
                StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                {
                    // No more matches
                    break;
                }
                count++;
            }
            return count;
        }

        private static char[] ExtractSeparators(string text)
        {
            HashSet<char> separators = new HashSet<char>();
            foreach (char character in text)
            {
                // If the character is not a letter,
                // then by definition it is a separator
                if (!char.IsLetter(character))
                {
                    separators.Add(character);
                }
            }
            return separators.ToArray();
        }

        public static HashSet<string> ExtractWords(string text)
        {
            HashSet<string> words = new HashSet<string>();
            char[] separators = ExtractSeparators(text);
            string[] allWords = text.Split(separators,
            StringSplitOptions.RemoveEmptyEntries);



            foreach (var word in allWords)
            {
                words.Add(word.ToLowerInvariant());
            }
            return words;
        }
    }
}