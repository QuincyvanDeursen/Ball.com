using OrderService.Domain;
using OrderService.Repository.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services
{
	public class ItemService : IItemService
	{
		private readonly IItemRepository _itemRepository;
		public ItemService(IItemRepository itemRepository)
		{
			_itemRepository = itemRepository;
		}
		public async Task<Item> Get(Guid id)
		{
			return await _itemRepository.GetByIdAsync(id);
		}
	}
}
