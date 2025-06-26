using OrderService.Domain;
using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace OrderService.Handlers
{
	public class PaymentCancelledHandler : IEventHandler<OrderCancelledEvent>
	{
		private readonly ILogger<PaymentCancelledHandler> _logger;
		private readonly IOrderService _orderService;
		public PaymentCancelledHandler(ILogger<PaymentCancelledHandler> logger, IOrderService orderService)
		{
			_logger = logger;
			_orderService = orderService;
		}
		public async Task HandleAsync(OrderCancelledEvent @event)
		{
			_logger.LogInformation("Handling PaymentCancelled event for OrderId: {OrderId}", @event.OrderId);

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
				PaymentStatus = PaymentStatus.Cancelled,
			};
			await _orderService.Update(orderUpdateDto);
			_logger.LogInformation("Payment cancelled successfully for OrderId: {OrderId}", @event.OrderId);

		}
	}
}
