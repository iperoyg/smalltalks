using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DateTimeDectector.Domain
{
    public interface IDateTimeDectector
    {
        List<DateTimeDectected> Detect(string input);
        Task<List<DateTimeDectected>> DetectAsync(string input, CancellationToken cancellationToken);
    }
}
