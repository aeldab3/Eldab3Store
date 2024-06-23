using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private StoreContext _Context;
        
        public ProductsController(StoreContext Context)
        {
            _Context = Context;
        }
            [HttpGet]
            public async Task <ActionResult<List<Product>>> GetProducts()
            {
                var products = await _Context.Products.ToListAsync();
                return Ok(products);
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<Product>> GetProduct(int id)
            {
                var product = await _Context.Products.SingleOrDefaultAsync(x => x.Id == id);
                return Ok(product);
            }
      
    }
}
