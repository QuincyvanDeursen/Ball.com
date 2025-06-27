using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Events.Items
{
    public class ItemUpdatedEvent : BaseEvent
    {
        public override string EventType => "item.updated";

        public Guid ItemId { get; init; }
        public string? Name { get; init; } = null;
        public string? Description { get; init; } = null;
        public decimal? Price { get; init; }

    }
}
