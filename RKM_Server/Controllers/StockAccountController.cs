using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockAccountController : Controller
    {
        private readonly StockAccountInterface _stockAccountInterface;
        private readonly IMapper _mapper;

        public StockAccountController(StockAccountInterface stockAccountInterface, IMapper mapper)
        {
            _stockAccountInterface = stockAccountInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<StockAccount>))]
        public IActionResult GetStockAccount()
        {
            var stockAccounts = _mapper.Map<List<StockAccountDto>>(_stockAccountInterface.GetStockAccounts());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stockAccounts);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(StockAccount))]
        [ProducesResponseType(400)]
        public IActionResult GetStockAccount(int id)
        {
            if (!_stockAccountInterface.StockAccountExist(id))
                return NotFound();

            var stockAccounts = _mapper.Map<StockAccountDto>(_stockAccountInterface.GetStockAccount(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(stockAccounts);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateStockAccount([FromBody] StockAccountDto stockAccountCreate)
        {
            if (stockAccountCreate == null)
                return BadRequest(ModelState);



            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stockAccountMap = _mapper.Map<StockAccount>(stockAccountCreate);


            if (!_stockAccountInterface.CreateStockAccount(stockAccountMap))
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
        public IActionResult UpdateStockAccount(int id, [FromBody] StockAccountDto updateStockAccount)
        {
            if (updateStockAccount == null)
                return BadRequest(ModelState);

            if (!_stockAccountInterface.StockAccountExist(id))
                return NotFound();

            var stockAccountMap = _mapper.Map<StockAccount>(updateStockAccount);

            if (!_stockAccountInterface.UpdateStockAccount(stockAccountMap))
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
        public IActionResult DeleteStockAccount(int id)
        {
            if (!_stockAccountInterface.StockAccountExist(id))
            {
                return NotFound();
            }

            var stockAccountToDelete = _stockAccountInterface.GetStockAccount(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_stockAccountInterface.DeleteStockAccount(stockAccountToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }


    }
}
