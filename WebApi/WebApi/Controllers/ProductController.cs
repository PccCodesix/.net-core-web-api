using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Dto;
using WebApi.Services;

namespace WebApi.Controllers
{
    // [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ProductController : Controller
    {
        private ILogger<ProductController> _logger;
        private readonly IMailService _mailService;



        public ProductController(ILogger<ProductController> logger, IMailService mailService)
        {
            _logger = logger;
            _mailService = mailService;

        }
        [HttpGet]
        public IActionResult GetProduct()
        {
            //return new JsonResult(new List<Product> {
            //new Product{ Id = 1,
            //    Name = "牛奶",
            //    Price = 2.5f
            // },
            //    new Product{ Id = 1,
            //        Name = "牛奶",
            //        Price = 2.5f
            // }
            //});

            //var temp = new JsonResult(ProductService.Current.Products)
            //{
            //    StatusCode = 200
            //};
            //return temp;
            return Ok(ProductService.Current.Products);
            //user like Ok Result Must  use IActionResult
            // use statusCode middleware

        }
        [Route("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(int id)
        {
            //return new JsonResult(ProductService.Current.Products.SingleOrDefault(x => x.Id == id));
            try
            {
                var product = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
                if (product == null)
                {

                    _logger.LogInformation($"Id为{id}的产品没有被找到..");
                    _mailService.Send("Product Deleted", $"Id为{id}的产品被删除了");
                    return NotFound();
                }
                return Ok(product);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"查找Id为{id}的产品时出现了错误!!!", ex) ;
                return StatusCode(500, "处理请求的时候发生了错误！");
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] ProductCreation product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            var maxId = ProductService.Current.Products.Max(x => x.Id);
            var newProduct = new Product
            {
                Id = ++maxId,
                Name = product.Name,
                Price = product.Price
            };
            ProductService.Current.Products.Add(newProduct);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);

        }
        public IActionResult Put(int id, [FromBody] ProductModification product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            model.Name = product.Name;
            model.Price = product.Price;
            model.Description = product.Description;
            // return Ok(model);
            return NoContent();

        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductModification> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            var toPatch = new ProductModification
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };
            patchDoc.ApplyTo(toPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (toPatch.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            model.Name = toPatch.Name;
            model.Description = toPatch.Description;
            model.Price = toPatch.Price;

            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            ProductService.Current.Products.Remove(model);
          
            return NoContent();
        }
    }
}