using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace ExecutorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftAdapter _shiftAdapter;

        public ShiftController(IShiftAdapter shiftAdapter)
        {
            _shiftAdapter = shiftAdapter;
        }

        [HttpGet]
        public IActionResult GetAllShifts()
        {
            var result = _shiftAdapter.GetAllShifts();
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
        public IActionResult GetShiftById(string id)
        {
            var result = _shiftAdapter.GetShiftById(id);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpGet("staff/{staffId}")]
        public IActionResult GetShiftsByStaffId(string staffId)
        {
            var result = _shiftAdapter.GetShiftsByStaffId(staffId);
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
        public IActionResult CreateShift([FromBody] ShiftVM shiftModel)
        {
            var result = _shiftAdapter.CreateShift(shiftModel);
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
        public IActionResult UpdateShift(string id, [FromBody] ShiftVM shiftModel)
        {
            var result = _shiftAdapter.UpdateShift(shiftModel);
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
        public IActionResult DeleteShift(string id)
        {
            var result = _shiftAdapter.RemoveShift(id);
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
