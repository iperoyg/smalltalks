using GrammarParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser.Models
{
    public class Terminal
    {

        public string Pattern { get; set; }
        public TerminalType Type { get; set; }
        public string Value { get; set; }
    }

    public enum TerminalType
    {
        Simple,
        Regex,
        Variable
    }
}
