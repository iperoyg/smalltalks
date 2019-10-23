using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GrammarParser.Models
{
    public class TerminalSequence
    {
        public IList<Terminal> Sequence { get; set; }
        public string ReadableSequence => string.Join(" ", Sequence.Select(s => s.Pattern));
    }
}
