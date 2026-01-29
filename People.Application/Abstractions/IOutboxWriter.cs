using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Application.Abstractions
{
    public interface IOutboxWriter
    {
        Task AddAsync(string type, object payload, CancellationToken ct);
    }
}
