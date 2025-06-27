using Microsoft.AspNetCore.Mvc;
using OrderService.Domain;
using OrderService.Dto;
using OrderService.Repository.Interfaces;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Interfaces;

namespace OrderService.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IItemRepository _itemRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IEventPublisher _eventPublisher;
		public OrderService(IOrderRepository orderRepository, IItemRepository itemRepository, ICustomerRepository customerRepository, IEventPublisher eventPublisher)
		{
			_orderRepository = orderRepository;
			_itemRepository = itemRepository;
			_customerRepository = customerRepository;
			_eventPublisher = eventPublisher;
		}
		public async Task Create(OrderCreateDto orderDto)
		{
			if (!await IsValidOrder(orderDto)) {
				throw new ArgumentException("Invalid Order");
			}

			//Convert Dto to Order
			var orderId = Guid.NewGuid();
			var order = new Order
			{
				OrderId = orderId,
				OrderDate = DateTime.Now,
				CustomerId = orderDto.CustomerId,
				OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
				{
					OrderId = orderId,
					ItemId = oi.ItemId,
					Quantity = oi.Quantity
				}).ToList()
			};
			List<OrderItemDto> orderItemDtos = new List<OrderItemDto>();

			var totalPrice = (decimal) 0;
			for (int i = 0; i < order.OrderItems.Count; i++) {
				var item = order.OrderItems[i];
				var price = await GetSnapshotPrice(item.ItemId);
				item.SnapshotPrice = price;
				totalPrice += price * item.Quantity;
				//Map items to Dto for Event
				orderItemDtos.Add(new OrderItemDto
				{ 
					ItemId = item.ItemId, 
					OrderQuantity = item.Quantity, 
					Price = item.SnapshotPrice
					});
			}
			order.TotalPrice = totalPrice;

			var customer = await GetCustomer(orderDto.CustomerId);
			var customerSnapshot = new CustomerSnapshot
			{
				FirstName = customer.FirstName, 
				LastName = customer.LastName,
				PhoneNumber = customer.PhoneNumber,
				Address = customer.Address,
				Email = customer.Email
			};
			order.Customer = customerSnapshot;

			await _orderRepository.CreateAsync(order);

			// Maak event aan en publiceer
			var orderPlacedEvent = new OrderPlacedEvent
			{
				OrderId = order.OrderId,
				CustomerId = order.CustomerId,
				OrderDate = order.OrderDate,
				TotalPrice = order.TotalPrice,
				FirstName = order.Customer.FirstName,
				LastName = order.Customer.LastName,
				Email = order.Customer.Email,
				Address = order.Customer.Address,
				PhoneNumber = order.Customer.PhoneNumber,
				Items = orderItemDtos
			};

		await _eventPublisher.PublishAsync(orderPlacedEvent);
		}

		public async Task<Order> Get(Guid id)
		{
			return await _orderRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<Order>> GetAll()
		{
			return await _orderRepository.GetAllAsync();
		}

		public async Task Update(OrderUpdateDto orderDto)
		{
			// 1. Retrieve the old payment and update the status
			var order = await _orderRepository.GetByIdAsync(orderDto.OrderId);
			if (order == null) throw new KeyNotFoundException($"Order with ID {orderDto.OrderId} not found.");

			// 1.1 Update order status (pending -> completed | cancelled)
			if (order.OrderStatus is OrderStatus.Completed or OrderStatus.Cancelled)
				throw new InvalidOperationException("Order Status cannot be updated after it has been paid or cancelled.");

			if (orderDto.OrderStatus != null)
			{
				order.OrderStatus = (OrderStatus) orderDto.OrderStatus;
				if (orderDto.OrderStatus == OrderStatus.Cancelled){

					List<OrderItemDto> orderItemDtos = new List<OrderItemDto>();
					foreach (OrderItem item in order.OrderItems)
					{
						orderItemDtos.Add(new OrderItemDto 
						{ 
							ItemId = item.ItemId, 
							OrderQuantity = item.Quantity, 
							Price = item.SnapshotPrice 
						});
					}
					//Send event -> Order cancelled
					var orderCancelledEvent = new OrderCancelledEvent
					{
						OrderId = order.OrderId,
						CustomerId = order.CustomerId,
						OrderDate = order.OrderDate,
						TotalPrice = order.TotalPrice,
						FirstName = order.Customer.FirstName,
						LastName = order.Customer.LastName,
						Email = order.Customer.Email,
						Address = order.Customer.Address,
						PhoneNumber = order.Customer.PhoneNumber,
						Items = orderItemDtos
					};

					await _eventPublisher.PublishAsync(orderCancelledEvent);
				}
			}

			// 1.2 Update payment status (pending -> completed | cancelled)
			if (order.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Cancelled)
				throw new InvalidOperationException("Order Payment Status cannot be updated after it has been paid or cancelled.");

			if (orderDto.PaymentStatus != null)
			{
				order.PaymentStatus = (PaymentStatus) orderDto.PaymentStatus;
			}

			// 1.3 Update the payment in the database
			await _orderRepository.UpdateAsync(order);
		}

		public async Task<decimal> GetSnapshotPrice(Guid itemId)
		{
			var item = await _itemRepository.GetByIdAsync(itemId);
			return item.Price;
		}

		public async Task<Customer> GetCustomer(Guid customerId)
		{
			return await _customerRepository.GetByIdAsync(customerId);
		}

		public async Task<Boolean> IsValidOrder(OrderCreateDto orderCreateDto)
		{
			//Check total quantity of order items <= 20
			int totalQuantity = 0;
			Dictionary<Guid, int> items = new Dictionary<Guid, int>();
			foreach (OrderItemCreateDto item in orderCreateDto.OrderItems)
			{
				//If the item is already in the dictionary, add item quantity to existing value
				//If the item does not exist in dictionary, add it with a default value of "0 + item quantity"
				items[item.ItemId] = items.GetValueOrDefault(item.ItemId) + item.Quantity;
				totalQuantity += item.Quantity;

			}
			if (totalQuantity > 20)
			{
				throw new ArgumentException("You can only order a maximum of 20 items at a time");
			}

			//For every item in the dictionary, look up the current stock and see if this order can be completed
			foreach (var (itemId, quantity) in items)
			{
				Item realItem = await _itemRepository.GetByIdAsync(itemId);
				if (quantity > realItem.Stock)
				{
					throw new ArgumentException("Inventory does not have enough items to complete this order");
				}
			}
			return true;
		}
	}
}
