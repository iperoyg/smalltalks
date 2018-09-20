using System;
using System.Collections.Generic;
using System.Text;

namespace DateTimeDectector.Domain
{
    public interface IDateTimeDectector
    {
        void Detect(string input);
    }
}
