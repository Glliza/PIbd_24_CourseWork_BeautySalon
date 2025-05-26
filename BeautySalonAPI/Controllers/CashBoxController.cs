using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;
using BeautySalon.AdapterContracts.OperationResponses;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashBoxController : ControllerBase
    {
        private readonly ICashBoxAdapter _cashBoxAdapter;

        public CashBoxController(ICashBoxAdapter cashBoxAdapter)
        {
            _cashBoxAdapter = cashBoxAdapter;
        }

        [HttpGet]
        public IActionResult GetAllCashBoxes()
        {
            var result = _cashBoxAdapter.GetAllCashBoxes();
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetCashBoxById(string id)
        {
            var result = _cashBoxAdapter.GetCashBoxById(id);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateCashBox([FromBody] CashBoxVM cashBoxModel)
        {
            var result = _cashBoxAdapter.CreateCashBox(cashBoxModel);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCashBox(string id, [FromBody] CashBoxVM cashBoxModel)
        {
            var result = _cashBoxAdapter.UpdateCashBox(cashBoxModel);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCashBox(string id)
        {
            var result = _cashBoxAdapter.DeleteCashBox(id);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return NoContent();
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }
    }
}
