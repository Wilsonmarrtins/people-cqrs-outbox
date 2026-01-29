using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Application.Commands.People.UpdatePerson
{
    public sealed record UpdatePersonResult(bool NotFound);
}
