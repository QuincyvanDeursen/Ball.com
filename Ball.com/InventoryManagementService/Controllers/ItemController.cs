using InventoryManagementService.Commands;
using InventoryManagementService.Commands.Handlers;
using InventoryManagementService.Models;
using InventoryManagementService.Queries;
using InventoryManagementService.Queries.Handlers;
using InventoryManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementService.Controllers
{
    // Controllers/ItemController.cs
    [ApiController]
    [Route("api/items")]
    public class ItemController : ControllerBase
    {
        private readonly ICommandHandler<CreateItemCommand> _createHandler;
        private readonly ICommandHandler<UpdateStockCommand> _updateStockHandler;
        private readonly IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>> _getAllHandler;
        private readonly IQueryHandler<GetItemsByIdQuery, ItemReadModel?> _getByIdHandler;
        private readonly IEventReplayer _replayer;

        public ItemController(
            ICommandHandler<CreateItemCommand> createHandler,
            ICommandHandler<UpdateStockCommand> updateStockHandler,
            IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>> getAllHandler,
            IQueryHandler<GetItemsByIdQuery, ItemReadModel?> getByIdHandler,
            IEventReplayer replayer)
        {
            _createHandler = createHandler;
            _updateStockHandler = updateStockHandler;
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _replayer = replayer;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
        {
            await _createHandler.HandleAsync(command);
            return Ok();
        }

        [HttpPost("stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
        {
            await _updateStockHandler.HandleAsync(command);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllItemsQuery();
            var products = await _getAllHandler.HandleAsync(query);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetItemsByIdQuery { Id = id };
            var product = await _getByIdHandler.HandleAsync(query);

            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost("replay")]
        public async Task<IActionResult> Replay()
        {
            await _replayer.ReplayAsync();
            return Ok("Replay completed.");
        }
    }
}
