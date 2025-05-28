using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace GuarantorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerAdapter _customerAdapter;

        public CustomerController(ICustomerAdapter customerAdapter)
        {
            _customerAdapter = customerAdapter;
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var result = _customerAdapter.GetAllCustomers();
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
        public IActionResult GetCustomerById(string id)
        {
            var result = _customerAdapter.GetCustomerById(id);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpGet("phone/{phoneNumber}")]
        public IActionResult GetCustomerByPhoneNumber(string phoneNumber)
        {
            var result = _customerAdapter.GetCustomerByPhoneNumber(phoneNumber);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpGet("fio/{fio}")]
        public IActionResult GetCustomerByFIO(string fio)
        {
            var result = _customerAdapter.GetCustomerByFIO(fio);
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
        public IActionResult RegisterCustomer([FromBody] CustomerVM customerModel)
        {
            var result = _customerAdapter.RegisterCustomer(customerModel);
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
        public IActionResult ChangeCustomerInfo(string id, [FromBody] CustomerVM customerModel)
        {
            var result = _customerAdapter.ChangeCustomerInfo(customerModel);
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
        public IActionResult RemoveCustomer(string id)
        {
            var result = _customerAdapter.RemoveCustomer(id);
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
