using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Entities.Core.Extensions
{

    public static class StringExtension
    {
        public static Dictionary<string, int> NumDict1 => GetNumDict(1);
        public static Dictionary<string, int> NumDict2 => GetNumDict(2);
        public static Dictionary<string, int> NumDict10 => GetNumDict(3);
        public static Dictionary<string, int> NumDict100 => GetNumDict(4);
        public static Dictionary<string, int> ScaleDict => GetScaleDict();
        public static Dictionary<string, int> OrderDict => GetOrderDict();



        private static Dictionary<string, int> _numLevel1Dict = null;
        private static Dictionary<string, int> _numLevel2Dict = null;
        private static Dictionary<string, int> _numLevel3Dict = null;
        private static Dictionary<string, int> _numLevel4Dict = null;
        private static Dictionary<string, int> _scaleDict = null;
        private static Dictionary<string, int> _orderDict = null;


        //Credit to: https://codereview.stackexchange.com/questions/119519/regex-to-first-match-then-replace-found-matches
        public static string Replace(this string s, int index, int length, string replacement)
        {
            var builder = new StringBuilder();
            builder.Append(s.Substring(0, index));
            builder.Append(replacement);
            builder.Append(s.Substring(index + length));
            return builder.ToString();
        }

        //Credit to: https://codereview.stackexchange.com/questions/119519/regex-to-first-match-then-replace-found-matches
        public static string Replace(this string s, int index, int length, char replacement)
        {
            var builder = new StringBuilder();
            builder.Append(s.Substring(0, index));
            builder.Append(replacement, length);
            builder.Append(s.Substring(index + length));
            return builder.ToString();
        }

        //Credit to: https://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string[] SplitTokens(this string input)
        {
            var chars = input.ToCharArray();
            var tokens = new List<string>();
            var token = new StringBuilder();
            foreach (var ch in chars)
            {
                var chs = $"{ch}";
                if (Regex.IsMatch(chs, "[\\.,\\:;\\\\\\/]")) // Separator isn't a white-space char
                {
                    ProcessAndInsertToken(tokens, token, chs);
                }
                else if (Regex.IsMatch(chs, "\\s"))
                {
                    ProcessAndInsertToken(tokens, token, " ");
                }
                else
                {
                    token.Append(chs);
                }
            }
            ProcessAndInsertToken(tokens, token, string.Empty);

            return tokens.ToArray();
        }

        //Credit to: https://pt.stackoverflow.com/questions/263365/n%C3%BAmero-em-extenso-para-n%C3%BAmero (adapted)
        public static string ParseIntegersFromRaw(this string raw)
        {
            var parsedString = new StringBuilder(raw.Length);
            var acc = 0;
            var currentGroup = 0;
            var isAcc = false;
            var andToken = "e";
            var lastWasAndToken = false;
            var lastWasSpace = false;
            var level = 1000;
            var isScaling = false;

            var tokens = raw.SplitTokens();
            foreach (var token in tokens)
            {
                if (string.IsNullOrWhiteSpace(token) && (isAcc || isScaling))
                {
                    lastWasSpace = true;
                    continue;
                }

                if (NumDict1.ContainsKey(token))
                {
                    if (level <= 3)
                    {
                        NumericHandlePreviousToken(parsedString, ref acc, ref currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level);
                    }
                    currentGroup += NumDict1[token];
                    isAcc = false;
                    isScaling = true;
                    lastWasAndToken = false;
                    level = 1;
                }
                else if (NumDict2.ContainsKey(token))
                {
                    if (level <= 3 || level == 10)
                    {
                        NumericHandlePreviousToken(parsedString, ref acc, ref currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level);
                    }
                    currentGroup += NumDict2[token];
                    isAcc = false;
                    isScaling = true;
                    lastWasAndToken = false;
                    level = 2;
                }
                else if (NumDict10.ContainsKey(token))
                {
                    if (level == 10 || level <= 3)
                    {
                        NumericHandlePreviousToken(parsedString, ref acc, ref currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level);
                    }
                    currentGroup += NumDict10[token];
                    isAcc = true;
                    isScaling = true;
                    lastWasAndToken = false;
                    level = 10;
                }
                else if (token.Equals("cem"))
                {
                    NumericHandlePreviousToken(parsedString, ref acc, ref currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level);
                    currentGroup = 100;
                    isAcc = false;
                    isScaling = true;
                    lastWasAndToken = false;
                    level = 3;
                }
                else if (NumDict100.ContainsKey(token))
                {
                    if (level == 100 || level == 3)
                    {
                        NumericHandlePreviousToken(parsedString, ref acc, ref currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level);
                    }
                    currentGroup += NumDict100[token];
                    isAcc = true;
                    isScaling = true;
                    lastWasAndToken = false;
                    level = 100;
                }
                else if (ScaleDict.ContainsKey(token))
                {
                    acc += (currentGroup == 0 ? 1 : currentGroup) * ScaleDict[token];
                    currentGroup = 0;
                    isAcc = true;
                    isScaling = false;
                    lastWasAndToken = false;
                    level = 1000;
                }
                else if (OrderDict.ContainsKey(token))
                {
                    if (isAcc)
                    {
                        parsedString.Append(acc + currentGroup);
                        parsedString.Append(" ");
                    }
                    parsedString.Append(OrderDict[token]);
                    //parsedString.Append(" ");
                    lastWasAndToken = false;
                    currentGroup = 0;
                    acc = 0;
                    isAcc = false;
                    isScaling = false;
                    level = 1000;

                }
                else if (token.Equals(andToken, StringComparison.OrdinalIgnoreCase))
                {
                    if (!lastWasAndToken)
                    {
                        lastWasAndToken = true;
                    }
                    else
                    {
                        lastWasAndToken = false;
                        parsedString.Append(acc + currentGroup);
                        parsedString.Append(" ");
                        parsedString.Append(andToken);
                        currentGroup = 0;
                        acc = 0;
                        isScaling = false;
                    }
                }
                else
                {
                    HandlePreviousToken(parsedString, currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level, acc);

                    parsedString.Append(token);

                    lastWasAndToken = false;
                    isAcc = false;
                    isScaling = false;
                    currentGroup = 0;
                    acc = 0;
                    level = 1000;
                }
                lastWasSpace = false;
            }

            HandlePreviousToken(parsedString, currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level, acc);

            return parsedString.ToString().Trim();
        }

        private static void NumericHandlePreviousToken(StringBuilder parsedString, ref int acc, ref int currentGroup, bool isAcc, string andToken, bool lastWasAndToken, bool lastWasSpace, int level)
        {
            HandlePreviousToken(parsedString, currentGroup, isAcc, andToken, lastWasAndToken, lastWasSpace, level, acc);
            //parsedString.Append(token);
            currentGroup = 0;
            acc = 0;
        }

        private static void InsertCurrentGroupValue(StringBuilder parsedString, int currentGroup, int acc)
        {
            parsedString.Append(acc + currentGroup);
        }

        private static void HandlePreviousToken(StringBuilder parsedString, int currentGroup, bool isAcc, string andToken, bool lastWasAndToken, bool lastWasSpace, int level, int acc)
        {
            if ((isAcc || level < 1000) && lastWasAndToken)
            {
                parsedString.Append(acc + currentGroup);
                parsedString.Append(" ");
                parsedString.Append(andToken);
            }
            else if ((isAcc || level < 1000))
            {
                parsedString.Append(acc + currentGroup);
            }
            else if (lastWasAndToken)
            {
                parsedString.Append(andToken);
            }
            if (lastWasSpace)
                parsedString.Append(" ");
        }

        private static void ProcessAndInsertToken(List<string> tokens, StringBuilder token, string chs)
        {
            if (token.Length > 0)
            {
                var ts = token.ToString();
                if (Regex.IsMatch(ts, "^[0-9]+$"))
                {
                    ts = NumberToString(Convert.ToInt32(ts));
                    ts = ts.Trim();
                    tokens.AddRange(ts.SplitTokens());
                }
                else
                {
                    tokens.Add(ts);
                }
            }
            if(!string.IsNullOrEmpty(chs))
                tokens.Add(chs);
            token.Clear();
        }
        private static string NumberToString(int num)
        {
            return NumberToTextParserBR.NumeroParaExtenso(num);
        }

        private static Dictionary<string, int> GetNumDict(int level)
        {
            var dict = new Dictionary<string, int>();

            if (level == 1)
            {
                if (_numLevel1Dict == null)
                {
                    _numLevel1Dict = new Dictionary<string, int>
                        {
                            { "zero", 0 },
                            { "um", 1 },
                            { "dois", 2 },
                            { "tres", 3 },
                            { "quatro", 4 },
                            { "cinco", 5 },
                            { "seis", 6 },
                            { "sete", 7 },
                            { "oito", 8 },
                            { "nove", 9 },
                        };

                }
                dict = _numLevel1Dict;
            }
            if (level == 2)
            {
                if (_numLevel2Dict == null)
                {
                    _numLevel2Dict = new Dictionary<string, int>
                        {
                            { "dez", 10 },
                            { "onze", 11 },
                            { "doze", 12 },
                            { "treze", 13 },
                            { "quatorze", 14 },
                            { "quinze", 15 },
                            { "dezesseis", 16 },
                            { "dezessete", 17 },
                            { "dezoito", 18 },
                            { "dezenove", 19 },
                        };
                }
                dict = _numLevel2Dict;
            }
            if (level == 3)
            {
                if (_numLevel3Dict == null)
                {
                    _numLevel3Dict = new Dictionary<string, int>
                        {
                            { "vinte", 20 },
                            { "trinta", 30 },
                            { "quarenta", 40 },
                            { "cinquenta", 50 },
                            { "sessenta", 60 },
                            { "setenta", 70 },
                            { "oitenta", 80 },
                            { "noventa", 90 },
                        };
                }
                dict = _numLevel3Dict;
            }
            if (level == 4)
            {
                if (_numLevel4Dict == null)
                {
                    _numLevel4Dict = new Dictionary<string, int>
                        {
                            { "cento", 100 },
                            { "duzentos", 200 },
                            { "duzentas", 200 },
                            { "trezentos", 300 },
                            { "trezentas", 300 },
                            { "quatrocentos", 400 },
                            { "quatrocentas", 400 },
                            { "quinhentos", 500 },
                            { "quinhentas", 500 },
                            { "seiscentos", 600 },
                            { "seiscentas", 600 },
                            { "setecentos", 700 },
                            { "setecentas", 700 },
                            { "oitocentos", 800 },
                            { "oitocentas", 800 },
                            { "novecentos", 900 },
                            { "novecentas", 900 }
                        };
                }
                dict = _numLevel4Dict;
            }

            return dict;
        }
        private static Dictionary<string, int> GetScaleDict()
        {
            if (_scaleDict == null)
            {
                _scaleDict = new Dictionary<string, int>
                {
                    { "mil", 1000 },
                    { "milhares", 1000 },
                    { "milhao", 1000000 },
                    { "milhoes", 1000000 },
                    { "bilhao", 1000000000 },
                    { "bilhoes", 1000000000 }
                };
            }
            return _scaleDict;
        }
        private static Dictionary<string, int> GetOrderDict()
        {
            if (_orderDict == null)
            {
                _orderDict = new Dictionary<string, int>
                {
                    { "primeiro", 1 },
                    { "segundo", 2 },
                    { "terceiro", 3 },
                    { "quarto", 4 },
                    { "quinto", 5 },
                    { "sexto", 6 },
                    { "setimo", 7 },
                    { "oitavo", 8 },
                    { "nono", 9 },
                    { "decimo", 10 },
                };
            }
            return _orderDict;
        }
    }

}
