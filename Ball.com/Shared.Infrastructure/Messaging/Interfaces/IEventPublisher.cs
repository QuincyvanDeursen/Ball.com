using Shared.Infrastructure.Messaging.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event) where T : IEvent;
        Task PublishAsync<T>(T @event, string routingKey) where T : IEvent;
    }
}
