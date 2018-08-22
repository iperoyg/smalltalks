using SmallTalks.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SmallTalks.Core.Services
{
    public static class PreProcessorBuilder
    {


        public static PreProcess RemoveRepeteadChars(this PreProcess preProcessor)
        {
            var input = preProcessor.Input.ToCharArray();
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


            }

            preProcessor.Output = builder.ToString();

            return preProcessor;
            

        }

        private static bool MatchesSingleCharRule(char currentChar)
        {
            return string.IsNullOrWhiteSpace(currentChar.ToString()) ||
                Regex.IsMatch(currentChar.ToString(), "[^\\w\\s]");
        }
    }
}
