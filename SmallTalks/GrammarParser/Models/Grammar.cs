using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Domain.Extensions;
using Entities.Domain.Models;
using System.Text.RegularExpressions;
using GrammarParser.Models;

namespace GrammarParser.Models
{
    public class Grammar
    {
        public const string StartToken = ">";
        public const string SplitToken = " | ";
        public const string RuleStartVariableTokenSeparador = ":";
        public const string VariablePattern = "\\b[A-Z][A-Z\\d_]*\\b";
        public const string RegexTerminalPattern = "^r'(?<regex>.+)'$";

        public IList<Rule> Rules { get; set; }
        public IList<Rule> StartRules { get; set; }
        public ISet<Terminal> KnowTerminals { get; set; }
        public ISet<Terminal> KnowVariables { get; set; }

    }


}
