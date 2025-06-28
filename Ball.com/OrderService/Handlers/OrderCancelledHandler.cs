using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace OrderService.Handlers
{
	public class OrderCancelledHandler : IEventHandler<OrderCancelledEvent>
	{
		private readonly IOrderService _orderService;
		private readonly ILogger<OrderCancelledHandler> _logger;

		public OrderCancelledHandler(IOrderService orderService, ILogger<OrderCancelledHandler> logger)
		{
			_orderService = orderService;
			_logger = logger;
		}

		public async Task HandleAsync(OrderCancelledEvent @event)
		{
			if (await _orderService.Get(@event.OrderId) == null)
			{
				throw new KeyNotFoundException($"Order with ID {@event.OrderId} not found OrderService.");
			}

			OrderUpdateDto orderUpdateDto = new OrderUpdateDto
			{
				OrderId = @event.OrderId,
				OrderStatus = Domain.OrderStatus.Cancelled
			};
			await _orderService.Update(orderUpdateDto);

			_logger.LogInformation("Order.Cancelled event processed successfully for OrderId: {OrderId}", @event.OrderId);
		}
	}
}
