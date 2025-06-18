using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Events.Interfaces
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
        string EventType { get; }
    }
}
