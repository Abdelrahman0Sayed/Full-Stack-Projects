using api.Dtos.Account;
using api.Dtos.Product;
using api.Repository;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IAuthServices authService , IEmailService emailService , IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("SetProduct")]
        public async Task<IActionResult> SetProductAsync([FromBody] ProductRequestDto newProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.SetProduct(newProduct);
            if (response.Message != "")
                return BadRequest(response);

            return Ok();
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(string userEmail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.GetTransactions(userEmail);

            if (response == null)
                return BadRequest("Something Wrong");
            
            return Ok(response);
        }
    }
}