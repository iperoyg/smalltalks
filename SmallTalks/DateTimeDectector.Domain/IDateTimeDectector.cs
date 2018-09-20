using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeDectector.Domain
{
    public interface IDateTimeDectector
    {
        List<DateTimeDectected> Detect(string input);
    }
}
