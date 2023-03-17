using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : Controller
    {
        private readonly StockInterface _stockInterface;
        private readonly IMapper _mapper;

        public StockController(StockInterface stockInterface, IMapper mapper)
        {
            _stockInterface = stockInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Stock>))]
        public IActionResult GetStock()
        {
            var stocks = _mapper.Map<List<StockDto>>(_stockInterface.GetStocks());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Stock))]
        [ProducesResponseType(400)]
        public IActionResult GetStock(int id)
        {
            if (!_stockInterface.StockExist(id))
                return NotFound();

            var stocks = _mapper.Map<StockDto>(_stockInterface.GetStock(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stocks);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateStock([FromBody] StockDto stockCreate)
        {
            if (stockCreate == null)
                return BadRequest(ModelState);



            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockMap = _mapper.Map<Stock>(stockCreate);


            if (!_stockInterface.CreateStock(stockMap))
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
        public IActionResult UpdateStock(int id, [FromBody] StockDto updateStock)
        {
            if (updateStock == null)
                return BadRequest(ModelState);

            if (!_stockInterface.StockExist(id))
                return NotFound();

            var stockMap = _mapper.Map<Stock>(updateStock);

            if (!_stockInterface.UpdateStock(stockMap))
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
        public IActionResult DeleteStock(int id)
        {
            if (!_stockInterface.StockExist(id))
            {
                return NotFound();
            }

            var stockToDelete = _stockInterface.GetStock(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_stockInterface.DeleteStock(stockToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }


    }
}
