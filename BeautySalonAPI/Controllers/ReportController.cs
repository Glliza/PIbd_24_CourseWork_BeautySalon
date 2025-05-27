using BeautySalon.BLImplementations;
using BeautySalon.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportGenerator _reportGenerator;

        public ReportController(ReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator;
        }

        [HttpPost("generate-staff-report")]
        public async Task<IActionResult> GenerateStaffReport([FromBody] List<Staff> staffList)
        {
            if (staffList == null || !staffList.Any())
            {
                return BadRequest("No staff data provided.");
            }

            var filePath = await _reportGenerator.GenerateStaffReport(staffList);
            return Ok(new { FilePath = filePath });
        }

        [HttpPost("generate-product-report")]
        public async Task<IActionResult> GenerateProductReport([FromBody] List<Product> productList)
        {
            if (productList == null || !productList.Any())
            {
                return BadRequest("No product data provided.");
            }

            var filePath = await _reportGenerator.GenerateProductReport(productList);
            return Ok(new { FilePath = filePath });
        }

        [HttpPost("generate-service-report")]
        public async Task<IActionResult> GenerateServiceReport([FromBody] List<Service> serviceList)
        {
            if (serviceList == null || !serviceList.Any())
            {
                return BadRequest("No service data provided.");
            }

            var filePath = await _reportGenerator.GenerateServiceReport(serviceList);
            return Ok(new { FilePath = filePath });
        }

        [HttpPost("generate-visit-report")]
        public async Task<IActionResult> GenerateVisitReport([FromBody] List<Visit> visitList)
        {
            if (visitList == null || !visitList.Any())
            {
                return BadRequest("No visit data provided.");
            }

            var filePath = await _reportGenerator.GenerateVisitReport(visitList);
            return Ok(new { FilePath = filePath });
        }

        [HttpPost("generate-request-report")]
        public async Task<IActionResult> GenerateRequestReport([FromBody] List<Request> requestList)
        {
            if (requestList == null || !requestList.Any())
            {
                return BadRequest("No request data provided.");
            }

            var filePath = await _reportGenerator.GenerateRequestReport(requestList);
            return Ok(new { FilePath = filePath });
        }
    }
}
