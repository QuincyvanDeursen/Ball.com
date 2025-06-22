using InventoryManagementService.Commands;
using InventoryManagementService.Commands.Handlers;
using InventoryManagementService.Models;
using InventoryManagementService.Queries;
using InventoryManagementService.Queries.Handlers;
using InventoryManagementService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementService.Controllers
{
    // Controllers/ProductController.cs
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ICommandHandler<CreateProductCommand> _createHandler;
        private readonly ICommandHandler<UpdateStockCommand> _updateStockHandler;
        private readonly IQueryHandler<GetAllProductsQuery, IEnumerable<ProductReadModel>> _getAllHandler;
        private readonly IQueryHandler<GetProductByIdQuery, ProductReadModel?> _getByIdHandler;
        private readonly IEventReplayer _replayer;

        public ProductController(
            ICommandHandler<CreateProductCommand> createHandler,
            ICommandHandler<UpdateStockCommand> updateStockHandler,
            IQueryHandler<GetAllProductsQuery, IEnumerable<ProductReadModel>> getAllHandler,
            IQueryHandler<GetProductByIdQuery, ProductReadModel?> getByIdHandler,
            IEventReplayer replayer)
        {
            _createHandler = createHandler;
            _updateStockHandler = updateStockHandler;
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _replayer = replayer;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
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
            var query = new GetAllProductsQuery();
            var products = await _getAllHandler.HandleAsync(query);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetProductByIdQuery { Id = id };
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
