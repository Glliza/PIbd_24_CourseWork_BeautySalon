using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptAdapter _receiptAdapter;

        public ReceiptController(IReceiptAdapter receiptAdapter)
        {
            _receiptAdapter = receiptAdapter;
        }

        [HttpGet]
        public IActionResult GetAllReceipts()
        {
            var result = _receiptAdapter.GetAllReceipts();
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
        public IActionResult GetReceiptById(string id)
        {
            var result = _receiptAdapter.GetReceiptById(id);
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
        public IActionResult CreateReceipt([FromBody] ReceiptVM receiptModel)
        {
            var result = _receiptAdapter.CreateReceipt(receiptModel);
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
        public IActionResult UpdateReceipt(string id, [FromBody] ReceiptVM receiptModel)
        {
            var result = _receiptAdapter.UpdateReceipt(receiptModel);
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
        public IActionResult DeleteReceipt(string id)
        {
            var result = _receiptAdapter.DeleteReceipt(id);
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
