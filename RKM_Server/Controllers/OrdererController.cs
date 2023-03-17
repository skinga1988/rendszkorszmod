using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdererController : Controller
    {
        private readonly OrdererInterface _ordererInterface;
        private readonly IMapper _mapper;

        public OrdererController(OrdererInterface ordererInterface, IMapper mapper)
        {
            _ordererInterface = ordererInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Orderer>))]
        public IActionResult GetOrderer()
        {
            var orderers = _mapper.Map<List<OrdererDto>>(_ordererInterface.GetOrderers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(orderers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Orderer))]
        [ProducesResponseType(400)]
        public IActionResult GetOrderer(int id)
        {
            if (!_ordererInterface.OrdererExist(id))
                return NotFound();

            var orderer = _mapper.Map<OrdererDto>(_ordererInterface.GetOrderer(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(orderer);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOrderer([FromBody] OrdererDto ordererCreate)
        {
            if (ordererCreate == null)
                return BadRequest(ModelState);

           

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ordererMap = _mapper.Map<Orderer>(ordererCreate);


            if (!_ordererInterface.CreateOrderer(ordererMap))
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateStockItem(int id, [FromBody] OrdererDto updatedOrderer)
        {
            if (updatedOrderer == null)
                return BadRequest(ModelState);

            if (!_ordererInterface.OrdererExist(id))
                return NotFound();

            var ordererMap = _mapper.Map<Orderer>(updatedOrderer);

            if (!_ordererInterface.UpdateOrderer(ordererMap))
            {
                ModelState.AddModelError("", "Update Error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOrderer(int id)
        {
            if (!_ordererInterface.OrdererExist(id))
            {
                return NotFound();
            }

            var ordererToDelete = _ordererInterface.GetOrderer(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ordererInterface.DeleteOrderer(ordererToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }


    }
}
