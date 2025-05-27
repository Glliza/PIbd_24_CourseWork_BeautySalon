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
        public async Task<IActionResult> GenerateProductReport([FromBody] ProductReportRequest request)
        {
            if (request.ProductList == null || !request.ProductList.Any())
            {
                return BadRequest("No product data provided.");
            }

            var filePath = await _reportGenerator.GenerateProductReport(request.MinPrice, request.MaxPrice, request.ProductList);
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
        public async Task<IActionResult> GenerateVisitReport([FromBody] VisitReportRequest request)
        {
            if (request.VisitList == null || !request.VisitList.Any())
            {
                return BadRequest("No visit data provided.");
            }

            var filePath = await _reportGenerator.GenerateVisitReport(request.StaffId, request.StartDate, request.EndDate, request.VisitList);
            return Ok(new { FilePath = filePath });
        }

        [HttpPost("generate-request-report")]
        public async Task<IActionResult> GenerateRequestReport([FromBody] RequestReportRequest request)
        {
            if (request.RequestList == null || !request.RequestList.Any())
            {
                return BadRequest("No request data provided.");
            }

            var filePath = await _reportGenerator.GenerateRequestReport(request.CustomerId, request.RequestList);
            return Ok(new { FilePath = filePath });
        }

        public class ProductReportRequest
        {
            public decimal MinPrice { get; set; }
            public decimal MaxPrice { get; set; }
            public List<Product> ProductList { get; set; }
        }

        public class VisitReportRequest
        {
            public string StaffId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<Visit> VisitList { get; set; }
        }

        public class RequestReportRequest
        {
            public string CustomerId { get; set; }
            public List<Request> RequestList { get; set; }
        }
    }
}
