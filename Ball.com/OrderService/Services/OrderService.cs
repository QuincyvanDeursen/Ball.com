using Microsoft.AspNetCore.Mvc;
using OrderService.Domain;
using OrderService.Dto;
using OrderService.Repository.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IItemRepository _itemRepository;
		private readonly ICustomerRepository _customerRepository;
		public OrderService(IOrderRepository orderRepository, IItemRepository itemRepository, ICustomerRepository customerRepository)
		{
			_orderRepository = orderRepository;
			_itemRepository = itemRepository;
			_customerRepository = customerRepository;
		}
		public async Task Create(OrderCreateDto orderDto)
		{
			//Check quantity of order items <= 20
			if (orderDto.OrderItems.Count > 20)
				throw new ArgumentException("You can only order a maximum of 20 items at a time");

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

			var totalPrice = (decimal) 0;
			for (int i = 0; i < order.OrderItems.Count; i++) {
				var item = order.OrderItems[i];
				var price = await GetSnapshotPrice(item.ItemId);
				item.SnapshotPrice = price;
				totalPrice += price * item.Quantity;
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
		}

		public async Task<Order> Get(Guid id)
		{
			return await _orderRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<Order>> GetAll()
		{
			return await _orderRepository.GetAllAsync();
		}

		public async Task Update(OrderUpdateDto order)
		{
			// 1. Retrieve the old payment and update the status
			var oldOrder = await _orderRepository.GetByIdAsync(order.OrderId);
			if (oldOrder == null) throw new KeyNotFoundException($"Order with ID {order.OrderId} not found.");

			// 1.1 Update order status (pending -> completed | cancelled)
			if (oldOrder.OrderStatus is OrderStatus.Completed or OrderStatus.Cancelled)
				throw new InvalidOperationException("Order Status cannot be updated after it has been paid or cancelled.");

			if (order.OrderStatus != null)
			{
				oldOrder.OrderStatus = (OrderStatus) order.OrderStatus;
			}

			// 1.2 Update payment status (pending -> completed | cancelled)
			if (oldOrder.PaymentStatus is PaymentStatus.Paid or PaymentStatus.Cancelled)
				throw new InvalidOperationException("Order Payment Status cannot be updated after it has been paid or cancelled.");

			if (order.PaymentStatus != null)
			{
				oldOrder.PaymentStatus = (PaymentStatus) order.PaymentStatus;
			}

			// 1.3 Update the payment in the database
			await _orderRepository.UpdateAsync(oldOrder);
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
	}
}
