using DateTimeDectector.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using DateTimeDectector.Domain;
using GrammarParser.Models;
using System.Linq;

namespace DateTimeDectector.Core.Services
{
    public class DateTimeDectectionEvaluator : IGrammarDateTimeDectectionEvaluator
    {
        public List<DateTimeDectected> Evaluate(List<EvaluatedRule> evaluatedRules)
        {
            return evaluatedRules.Select(r => new DateTimeDectected
            {
                DateTime = DateTime.Now,
                AnalysedText = r.Input,
                EvaluatedText = r.Value,
                DetectionType = DateTimeParserDetectionType.NOW,
            })
            .ToList();
        }
    }
}
