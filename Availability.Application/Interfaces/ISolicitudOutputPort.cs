using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Interfaces
{
    public interface ISolicitudOutputPort
    {
        Task PresentarResultado(Guid solicitudID, string estado, string menasje);
    }
}
