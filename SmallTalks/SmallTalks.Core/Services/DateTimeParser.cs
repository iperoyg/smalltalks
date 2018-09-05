using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Takenet.Textc;
using Takenet.Textc.Csdl;
using Takenet.Textc.PreProcessors;
using Takenet.Textc.Processors;

namespace SmallTalks.Core.Services
{
    public class DateTimeParser
    {
        public TextProcessor Build()
        {
            var todaySyntax = CsdlParser.Parse("[today:LDWord(hoje,hj)]");
            var tomorrowSyntax = CsdlParser.Parse("[tomorrow:LDWord?(amanha)]");
            var yesterdaySyntax = CsdlParser.Parse("[yesterday:LDWord?(ontem)]");

            MyDateTime myDateTime = new MyDateTime();

            var todayCommandProcessor = new ReflectionCommandProcessor(
                myDateTime,
                nameof(myDateTime.Add),
                true,
                outputProcessor: null,
                syntaxes: todaySyntax);
            // 5. Register the the processors
            var textProcessor = new TextProcessor();
            textProcessor.CommandProcessors.Add(todayCommandProcessor);

            // 6. Add some preprocessors to normalize the input text
            textProcessor.TextPreprocessors.Add(new TextNormalizerPreprocessor());
            textProcessor.TextPreprocessors.Add(new ToLowerCasePreprocessor());
            return textProcessor;
        }

    }

    public class MyDateTime
    {
        public Task<string> Add(string today, IRequestContext context)
        {
            return Task.FromResult(today);
        }
    }

}
