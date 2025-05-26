using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestAdapter _requestAdapter;

        public RequestController(IRequestAdapter requestAdapter)
        {
            _requestAdapter = requestAdapter;
        }

        [HttpGet]
        public IActionResult GetAllRequests()
        {
            var result = _requestAdapter.GetAllRequests();
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
        public IActionResult GetRequestById(string id)
        {
            var result = _requestAdapter.GetRequestById(id);
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
        public IActionResult CreateRequest([FromBody] RequestVM requestModel)
        {
            var result = _requestAdapter.CreateRequest(requestModel);
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
        public IActionResult UpdateRequest(string id, [FromBody] RequestVM requestModel)
        {
            var result = _requestAdapter.UpdateRequest(requestModel);
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
        public IActionResult DeleteRequest(string id)
        {
            var result = _requestAdapter.DeleteRequest(id);
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
