using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser
{
    public class Rule
    { 
        public Terminal Variable { get; set; }
        public IEnumerable<TerminalSequence> TerminalsSequence { get; set; }
        public RuleType Type { get; set; }
    }

    public enum RuleType
    {
        Normal,
        Start
    }
}
