using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeDectector.Domain
{
    public class DateTimeDectected
    {
        public DateTime DateTime { get; set; }
        public string EvaluatedText { get; set; }
        public int EvaluationStartPosition { get; set; }
        public int EvaluationLenght { get; set; }
        public DateTimeParserDetectionType DetectionType { get; set; }
        public string AnalysedText { get; set; }
    }
}
