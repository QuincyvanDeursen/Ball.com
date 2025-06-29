using Microsoft.Identity.Client;
using OrderService.Domain;
using OrderService.Dto;
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

        public async Task Create(ItemCreateDto itemDto)
        {
            var item = new Item
            {
                ItemId = itemDto.ItemId,
                Name = itemDto.Name,
                Price = itemDto.Price,
                Stock = itemDto.Stock,
            };
            await _itemRepository.CreateAsync(item);
        }

        public async Task<Item> Get(Guid id)
        {
            return await _itemRepository.GetByIdAsync(id);
        }

        public async Task Update(ItemUpdateDto itemDto)
        {
            var olditem = await _itemRepository.GetByIdAsync(itemDto.ItemId);
            if (olditem == null) throw new KeyNotFoundException($"Item with ID {itemDto.ItemId} not found");

            olditem.ItemId = itemDto.ItemId;
            olditem.Name = itemDto.Name ?? olditem.Name;
            olditem.Price = itemDto.Price ?? olditem.Price;
            olditem.Stock = olditem.Stock;

            await _itemRepository.UpdateAsync(olditem);

        }
    }
}
