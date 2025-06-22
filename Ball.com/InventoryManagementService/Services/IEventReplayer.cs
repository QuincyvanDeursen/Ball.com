namespace InventoryManagementService.Services
{
    public interface IEventReplayer
    {
        Task ReplayAsync();
    }
}
