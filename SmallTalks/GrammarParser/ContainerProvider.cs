using GrammarParser.Interfaces;
using GrammarParser.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarParser
{
    public class ContainerProvider
    {
        public static IServiceCollection RegisterDefaults(IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IGrammarSourceProvider, LocalGrammarSourceProvider>()
                .AddSingleton<IGrammarParserService, MainGrammarParser>()
                .AddSingleton<IGrammarProvider, LocalGrammarProvider>()
                .AddSingleton<IRuleParserService, RuleParserService>()
                .AddSingleton<ITerminalParserService, TerminalParserService>();

        }
    }
}
