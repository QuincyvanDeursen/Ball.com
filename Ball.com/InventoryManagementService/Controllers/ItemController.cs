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
        private readonly ICommandHandler<UpdateItemCommand> _updateHandler;
        private readonly ICommandHandler<UpdateStockCommand> _updateStockHandler;
        private readonly IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>> _getAllHandler;
        private readonly IQueryHandler<GetItemsByIdQuery, ItemReadModel> _getByIdHandler;
        private readonly IEventReplayer _replayer;

        public ItemController(
            ICommandHandler<CreateItemCommand> createHandler,
            ICommandHandler<UpdateItemCommand> updateHandler,
            ICommandHandler<UpdateStockCommand> updateStockHandler,
            IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>> getAllHandler,
            IQueryHandler<GetItemsByIdQuery, ItemReadModel> getByIdHandler,
            IEventReplayer replayer)
        {
            _createHandler = createHandler ?? throw new ArgumentNullException(nameof(createHandler));
            _updateStockHandler = updateStockHandler ?? throw new ArgumentNullException(nameof(updateStockHandler));
            _getAllHandler = getAllHandler ?? throw new ArgumentNullException(nameof(getAllHandler));
            _getByIdHandler = getByIdHandler ?? throw new ArgumentNullException(nameof(getByIdHandler));
            _replayer = replayer ?? throw new ArgumentNullException(nameof(replayer));
            _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var query = new GetAllItemsQuery();
                var products = await _getAllHandler.HandleAsync(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving items: {ex.Message}");

            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {

                if (id == Guid.Empty) return BadRequest("Item ID is required.");
                var query = new GetItemsByIdQuery { Id = id };
                var product = await _getByIdHandler.HandleAsync(query);
                if (product == null) return NotFound();
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");

            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemCommand command)
        {
            try
            {
                if (command == null) return BadRequest("Command cannot be null.");
                if (string.IsNullOrWhiteSpace(command.Name)) return BadRequest("Item name is required.");
                if (string.IsNullOrWhiteSpace(command.Description)) return BadRequest("Description is required.");
                if (command.Price <= 0) return BadRequest("Item price must be greater than zero.");
                if (command.Stock < 0) return BadRequest("Initial stock cannot be negative.");
                await _createHandler.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing command: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateItemCommand command)
        {
            try
            {
                if (command == null) return BadRequest("Command cannot be null.");
                if (command.ItemId == Guid.Empty) return BadRequest("Item ID is required.");
                if (string.IsNullOrWhiteSpace(command.Name)) return BadRequest("Item name is required.");
                if (string.IsNullOrWhiteSpace(command.Description)) return BadRequest("Description is required.");
                if (command.Price <= 0 && command.Price != null) return BadRequest("Item price must be greater than zero.");
                await _updateHandler.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing command: {ex.Message}");
            }
        }

        [HttpPost("stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
        {
            try
            {
                if (command == null) return BadRequest("Command cannot be null.");
                if (command.ItemId == Guid.Empty) return BadRequest("Item ID is required.");
                await _updateStockHandler.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing command: {ex.Message}");
            }
        }

        [HttpPost("replay")]
        public async Task<IActionResult> Replay()
        {
            try
            {
                await _replayer.ReplayAsync();
                return Ok("Replay completed.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error during replay: {ex.Message}");
            }
        }
    }
}
