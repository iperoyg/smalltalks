using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser.Interfaces
{
    public interface ITerminalParserService
    {
        Terminal FromText(string input, TerminalType type = TerminalType.Simple);
        Terminal ExtractVariableFromStart(string input);
        Terminal ExtractVariableFromMiddle(string input);
    }
}
