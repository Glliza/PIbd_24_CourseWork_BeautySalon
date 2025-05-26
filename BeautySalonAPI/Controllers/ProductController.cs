using Microsoft.AspNetCore.Mvc;
using System.Net;
using BeautySalon.AdapterContracts;
using BeautySalon.ViewModels;

namespace BeautySalonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductAdapter _productAdapter;

        public ProductController(IProductAdapter productAdapter)
        {
            _productAdapter = productAdapter;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var result = _productAdapter.GetAllProducts();
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
        public IActionResult GetProductById(string id)
        {
            var result = _productAdapter.GetProductById(id);
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
        public IActionResult GetProductByName(string name)
        {
            var result = _productAdapter.GetProductByName(name);
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
        public IActionResult CreateProduct([FromBody] ProductVM productModel)
        {
            var result = _productAdapter.CreateProduct(productModel);
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
        public IActionResult UpdateProduct(string id, [FromBody] ProductVM productModel)
        {
            var result = _productAdapter.UpdateProduct(productModel);
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
        public IActionResult DeleteProduct(string id)
        {
            var result = _productAdapter.DeleteProduct(id);
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return NoContent();
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }

        [HttpPatch("{productId}/stock/{quantityChange}")]
        public IActionResult UpdateStockQuantity(string productId, int quantityChange)
        {
            var result = _productAdapter.UpdateStockQuantity(productId, quantityChange);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return Ok(result.Result);
            }
            else
            {
                return StatusCode((int)result.StatusCode, result.Message);
            }
        }
    }
}
