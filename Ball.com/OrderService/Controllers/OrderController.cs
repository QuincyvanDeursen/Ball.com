using Microsoft.AspNetCore.Mvc;
using OrderService.Dto;
using OrderService.Services.Interfaces;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var orders = await _orderService.GetAll();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _orderService.Get(id);
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto orderCreateDto)
        {
			try
			{
				await _orderService.Create(orderCreateDto);
				return Ok();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
		}

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderUpdateDto orderUpdateDto)
        {
			try
			{
				await _orderService.Update(orderUpdateDto);
				return Ok();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}

		}

    }
}
