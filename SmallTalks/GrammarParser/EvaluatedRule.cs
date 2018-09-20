using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser
{
    public class EvaluatedRule
    {
        public Rule Rule { get; set; }
        public string Value { get; set; }
        public string Input { get; internal set; }
    }
}
