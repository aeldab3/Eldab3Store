using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _repo;
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }
            [HttpGet]
            public async Task <ActionResult<List<Product>>> GetProducts()
            {
                var products = await _repo.GetProductsAsync();
                return Ok(products);
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<Product>> GetProduct(int id)
            {
                var product = await _repo.GetProductByIdAsync(id);
                return Ok(product);
            }

            [HttpGet("brands")]
            public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrand()
            {
                return Ok ( await _repo.GetBrandsAsync());
            }

            [HttpGet("types")]
            public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
            {
                return Ok(await _repo.GetTypesAsync());
            }
      
    }
}
