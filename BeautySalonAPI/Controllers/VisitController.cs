using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IVisitAdapter _visitAdapter;

        public VisitController(IVisitAdapter visitAdapter)
        {
            _visitAdapter = visitAdapter;
        }

        [HttpGet]
        public IActionResult GetAllVisits()
        {
            var result = _visitAdapter.GetAllVisits();
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
        public IActionResult GetVisitById(string id)
        {
            var result = _visitAdapter.GetVisitById(id);
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
        public IActionResult CreateVisit([FromBody] VisitVM visitModel)
        {
            var result = _visitAdapter.CreateVisit(visitModel);
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
        public IActionResult UpdateVisit(string id, [FromBody] VisitVM visitModel)
        {
            var result = _visitAdapter.UpdateVisit(visitModel);
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
        public IActionResult CancelVisit(string id)
        {
            var result = _visitAdapter.CancelVisit(id);
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
