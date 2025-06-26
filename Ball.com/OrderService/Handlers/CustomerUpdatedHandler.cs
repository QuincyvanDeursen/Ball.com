using OrderService.Dto;
using OrderService.Repository.Interfaces;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;

namespace OrderService.Handlers
{
	public class CustomerUpdatedHandler : IEventHandler<CustomerUpdatedEvent>
	{
		private readonly ICustomerService _customerService;
		private readonly ILogger<CustomerUpdatedHandler> _logger;

		public CustomerUpdatedHandler(ICustomerService customerService, ILogger<CustomerUpdatedHandler> logger)
		{
			_customerService = customerService;
			_logger = logger;
		}

		public async Task HandleAsync(CustomerUpdatedEvent @event)
		{
			_logger.LogInformation("Handling Customer.Updated event for CustomerId: {CustomerId}", @event.CustomerId);

			if (await _customerService.Get(@event.CustomerId) == null)
			{
				throw new KeyNotFoundException($"Customer with ID {@event.CustomerId} not found PaymentService.");
			}

			CustomerUpdateDto customerUpdateDto = new CustomerUpdateDto
			{
				CustomerId = @event.CustomerId,
				Address = @event.Address,
				PhoneNumber = @event.PhoneNumber,
				Email = @event.Email,
			};

			await _customerService.Update(customerUpdateDto);

			_logger.LogInformation("Customer.Updated event processed successfully for CustomerId: {CustomerId}", @event.CustomerId);
		}
	}
}
