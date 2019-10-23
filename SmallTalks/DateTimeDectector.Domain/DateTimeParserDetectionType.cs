using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeDectector.Domain
{
    public enum DateTimeParserDetectionType
    {
        TODAY,
        TOMORROW,
        YESTERDAY,
        SPECIFIC_DATE,
        RELATIVE_DATE,
        FUTURE_RELATIVE_DATE,
        PAST_RELATIVE_DATE,
        NOW,
        MORNING,
        AFTERNOON,
        NIGHT,
        DAWN,
        SPECIFIC_TIME,
        RELATIVE_TIME,
        FUTURE_RELATIVE_TIME,
        PAST_RELATIVE_TIME,
    }
}
