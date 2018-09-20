using Entities.Core.Extensions;
using Entities.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Entities.Core.Services
{
    public static class InputProcessorBuilder
    {
        public static Regex PunctuationRegex = new Regex("[.,\\/#!$%\\^&\\*;:{}=\\-_`~()]", RegexOptions.Compiled);

        public static InputProcess RemoveRepeteadChars(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            var builder = new StringBuilder();
            char firstLastChar = (char)0;
            char secondLastChar = (char)0;
            char currentChar = (char)0;
            for (int i = 0; i < input.Length; i++)
            {
                secondLastChar = firstLastChar;
                firstLastChar = currentChar;
                currentChar = input[i];

                if (MatchesSingleCharRule(currentChar) && currentChar == firstLastChar) continue;

                if (currentChar != secondLastChar)
                {
                    builder.Append(currentChar);
                }
                else if (currentChar == secondLastChar && secondLastChar != firstLastChar)
                {
                    builder.Append(currentChar);
                }
            }

            inputProcess.Output = builder.ToString();
            return inputProcess;
        }

        public static InputProcess ToLower(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            inputProcess.Output = input.ToLowerInvariant();
            return inputProcess;
        }

        public static InputProcess RemovePunctuation(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            inputProcess.Output = PunctuationRegex.Replace(input, string.Empty);
            return inputProcess;
        }

        public static InputProcess RemovePlaceholder(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            inputProcess.Output = input.Replace(InputProcess.Placeholder.ToString(), string.Empty).Trim();
            return inputProcess;
        }

        public static InputProcess RemoveAccentuation(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            inputProcess.Output = input.RemoveDiacritics();
            return inputProcess;
        }

        public static InputProcess TranslateTextNumberToNumeric(this InputProcess inputProcess)
        {
            var input = inputProcess.Data;
            inputProcess.Output = input.ParseIntegersFromRaw();
            return inputProcess;
        }

        private static bool MatchesSingleCharRule(char currentChar)
        {
            return string.IsNullOrWhiteSpace(currentChar.ToString()) ||
                Regex.IsMatch(currentChar.ToString(), "[^\\w\\s]") ||
                InputProcess.Non_Duplicable_Consonants.Contains(currentChar);
        }
    }
}
