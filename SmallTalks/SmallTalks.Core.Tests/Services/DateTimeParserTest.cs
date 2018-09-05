using NUnit.Framework;
using SmallTalks.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Takenet.Textc;

namespace SmallTalks.Core.Tests.Services
{
    [TestFixture]
    public class DateTimeParserTest
    {

        [TestCase("hoje")]
        public void Simple_DateTimeParserTest(string input)
        {
            var context = new RequestContext();
            var parser = new DateTimeParser();
            var processor = parser.Build();
            processor.ProcessAsync(input, context, CancellationToken.None).Wait();


        }

    }
}
