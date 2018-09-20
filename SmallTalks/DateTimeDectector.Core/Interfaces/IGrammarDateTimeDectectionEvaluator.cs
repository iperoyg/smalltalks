using DateTimeDectector.Domain;
using GrammarParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeDectector.Core.Interfaces
{
    public interface IGrammarDateTimeDectectionEvaluator
    {
        List<DateTimeDectected> Evaluate(List<EvaluatedRule> evaluatedRules);
    }
}
