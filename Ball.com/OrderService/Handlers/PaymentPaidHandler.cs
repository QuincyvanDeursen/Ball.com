using OrderService.Domain;
using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Events.Payments;

namespace OrderService.Handlers
{
	public class OrderPaidHandler : IEventHandler<PaymentPaidEvent>
	{
		private readonly ILogger<OrderPaidHandler> _logger;
		private readonly IOrderService _orderService;
		public OrderPaidHandler(ILogger<OrderPaidHandler> logger, IOrderService orderService)
		{
			_logger = logger;
			_orderService = orderService;
		}
		public async Task HandleAsync(PaymentPaidEvent @event)
		{
			_logger.LogInformation("Handling OrderPaid event for OrderId: {OrderId}", @event.OrderId);

			try
			{
				await _orderService.Get(@event.OrderId);
			}
			catch (KeyNotFoundException)
			{
				_logger.LogWarning("No order found for OrderId: {OrderId}", @event.OrderId);
				return;
			}

			OrderUpdateDto orderUpdateDto = new OrderUpdateDto
			{
				OrderId = @event.OrderId,
				OrderStatus = OrderStatus.Completed,
				PaymentStatus = PaymentStatus.Paid,
			};
			await _orderService.Update(orderUpdateDto);
			_logger.LogInformation("Order paid successfully for OrderId: {OrderId}", @event.OrderId);

		}
	}
}
