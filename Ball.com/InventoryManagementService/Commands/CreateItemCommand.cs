using System.Text.Json.Serialization;

namespace InventoryManagementService.Commands
{
    public class CreateItemCommand : ICommand
    {
        [JsonIgnore]
        public Guid ItemId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
