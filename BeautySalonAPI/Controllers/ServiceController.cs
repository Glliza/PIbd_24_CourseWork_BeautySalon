using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceAdapter _serviceAdapter;

        public ServiceController(IServiceAdapter serviceAdapter)
        {
            _serviceAdapter = serviceAdapter;
        }

        [HttpGet]
        public IActionResult GetAllServices()
        {
            var result = _serviceAdapter.GetAllServices();
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
        public IActionResult GetServiceById(string id)
        {
            var result = _serviceAdapter.GetServiceById(id);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpGet("name/{name}")]
        public IActionResult GetServiceByName(string name)
        {
            var result = _serviceAdapter.GetServiceByName(name);
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
        public IActionResult CreateService([FromBody] ServiceVM serviceModel)
        {
            var result = _serviceAdapter.CreateService(serviceModel);
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
        public IActionResult UpdateService(string id, [FromBody] ServiceVM serviceModel)
        {
            var result = _serviceAdapter.UpdateService(serviceModel);
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
        public IActionResult DeleteService(string id)
        {
            var result = _serviceAdapter.DeleteService(id);
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
