using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StockItemController : Controller
    {
        private readonly StockItemInterface _stockItemInterface;
        private readonly IMapper _mapper;

        public StockItemController(StockItemInterface stockItemInterface, IMapper mapper)
        {
            _stockItemInterface = stockItemInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StockItem>))]
        public IActionResult GetStockItems()
        {
            var stockitems = _mapper.Map<List<StockItemDto>>(_stockItemInterface.GetStockItems());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stockitems);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(StockItem))]
        [ProducesResponseType(400)]
        public IActionResult GetStockItem(int id)
        {
            if (!_stockItemInterface.StockItemExist(id))
                return NotFound();

            var stockitem = _mapper.Map<StockItemDto>(_stockItemInterface.GetStockItem(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stockitem);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateStockItem([FromBody] StockItemDto stockitemCreate)
        {
            if (stockitemCreate == null)
                return BadRequest(ModelState);

            var stockitem = _stockItemInterface.GetStockItems()
                .Where(c => c.ItemType.Trim().ToUpper() == stockitemCreate.ItemType.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (stockitem != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockitemMap = _mapper.Map<StockItem>(stockitemCreate);


            if (!_stockItemInterface.CreateStockItem(stockitemMap))
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
        public IActionResult UpdateStockItem(int id, [FromBody] StockItemDto updatedStockItem)
        {
            if (updatedStockItem == null)
                return BadRequest(ModelState);

            if (!_stockItemInterface.StockItemExist(id))
                return NotFound();

            var stockitemMap = _mapper.Map<StockItem>(updatedStockItem);

            if (!_stockItemInterface.UpdateStockItem(stockitemMap))
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
        public IActionResult DeleteStockItem(int id)
        {
            if (!_stockItemInterface.StockItemExist(id))
            {
                return NotFound();
            }

            var stockitemToDelete = _stockItemInterface.GetStockItem(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_stockItemInterface.DeleteStockItem(stockitemToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }


    }
}
