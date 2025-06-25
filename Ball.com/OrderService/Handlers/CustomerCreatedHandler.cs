using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace OrderService.Handlers
{
	public class CustomerCreatedHandler : IEventHandler<CustomerCreatedEvent>
	{
		private readonly ICustomerService _customerService;
		private readonly ILogger<CustomerCreatedHandler> _logger;

		public CustomerCreatedHandler(ICustomerService customerService, ILogger<CustomerCreatedHandler> logger)
		{
			_customerService = customerService;
			_logger = logger;
		}

		public async Task HandleAsync(CustomerCreatedEvent @event)
		{
			_logger.LogInformation("Handling Customer.Created event for Customerid: {CustomerId}", @event.CustomerId);

			CustomerCreateDto Customer;

			try
			{
				await _customerService.Get(@event.CustomerId);

				// Optional: Als de klant al bestaat, hoef je hem niet opnieuw aan te maken
				_logger.LogInformation("Customer with ID {CustomerId} already exists. Skipping creation.", @event.CustomerId);
				return;
			}
			catch (KeyNotFoundException)
			{
				Customer = new CustomerCreateDto
				{
					CustomerId = @event.CustomerId,
					PhoneNumber = @event.PhoneNumber,
					LastName = @event.LastName,
					FirstName = @event.FirstName,
					Address	= @event.Address,
					Email = @event.Email,
				};
			}

			await _customerService.Create(Customer);

			_logger.LogInformation("Customer.Created event processed successfully for Customerid: {CustomerId}", @event.CustomerId);
		}
	}
}
