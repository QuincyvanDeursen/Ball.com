using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace OrderService.Handlers
{
	public class OrderValidatedHandler : IEventHandler<OrderValidatedEvent>
	{
		private readonly IOrderService _orderService;
		private readonly ILogger<OrderValidatedHandler> _logger;

		public OrderValidatedHandler(IOrderService orderService, ILogger<OrderValidatedHandler> logger)
		{
			_orderService = orderService;
			_logger = logger;
		}

		public async Task HandleAsync(OrderValidatedEvent @event)
		{
			if (await _orderService.Get(@event.OrderId) == null)
			{
				throw new KeyNotFoundException($"Order with ID {@event.OrderId} not found OrderService.");
			}

			OrderUpdateDto orderUpdateDto = new OrderUpdateDto
			{
				OrderId = @event.OrderId,
				OrderStatus = Domain.OrderStatus.Validated
			};
			await _orderService.Update(orderUpdateDto);

			_logger.LogInformation("Order.Validated event processed successfully for OrderId: {OrderId}", @event.OrderId);
		}
	}
}
