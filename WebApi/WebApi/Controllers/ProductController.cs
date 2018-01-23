using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Dto;
using WebApi.Entity;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Controllers
{
    // [Produces("application/json")]
    [Route("api/[Controller]")]
    public class ProductController : Controller
    {
        private ILogger<ProductController> _logger;
        private readonly IMailService _mailService;
        private readonly IProductRepository _productRepository;



        public ProductController(ILogger<ProductController> logger, IMailService mailService, IProductRepository productRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _productRepository = productRepository;
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
            //return Ok(ProductService.Current.Products);
            //user like Ok Result Must  use IActionResult
            // use statusCode middleware

            var products = _productRepository.GetProducts();
            var result = new List<ProductWithoutMaterialDto>();
            //foreach (var product in products)
            //{
            //    result.Add(new ProductWithoutMaterialDto
            //    {
            //        Id = product.Id,
            //        Name = product.Name,
            //        Price = product.Price,
            //        Description = product.Description

            //    });
            //}
            var results = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);

            return Ok(result);

        }
        [Route("{id}", Name = "GetProduct")]
        public IActionResult GetProduct(int id, bool includeMaterial = false)
        {
            //return new JsonResult(ProductService.Current.Products.SingleOrDefault(x => x.Id == id));
            try
            {
                //var product = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
                //if (product == null)
                //{

                //    _logger.LogInformation($"Id为{id}的产品没有被找到..");
                //    _mailService.Send("Product Deleted", $"Id为{id}的产品被删除了");
                //    return NotFound();
                //}
                //return Ok(product);
                var product = _productRepository.GetProduct(id, includeMaterial);
                if (product == null)
                {
                    return NotFound();
                }
                if (includeMaterial)
                {
                    //var productWithMaterialResult = new ProductDto
                    //{
                    //    Id = product.Id,
                    //    Name = product.Name,
                    //    Price = product.Price,
                    //    Description = product.Description
                    //};
                    //foreach (var material in product.Materials)
                    //{
                    //    productWithMaterialResult.Materials.Add(new MaterialDto
                    //    {
                    //        Id = material.Id,
                    //        Name = material.Name
                    //    });
                    // }
                    var productWithMaterialResult = Mapper.Map<ProductDto>(product);

                    return Ok(productWithMaterialResult);
                }

                //var onlyProductResult = new ProductDto
                //{
                //    Id = product.Id,
                //    Name = product.Name,
                //    Price = product.Price,
                //    Description = product.Description
                //};
                var onlyProductResult = Mapper.Map<ProductWithoutMaterialDto>(product);

                return Ok(onlyProductResult);
            }

            
            catch (Exception ex)
            {
                _logger.LogCritical($"查找Id为{id}的产品时出现了错误!!!", ex);
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
            //var newProduct = new Product
            //{
            //    Id = ++maxId,
            //    Name = product.Name,
            //    Price = product.Price
            //};
            // ProductService.Current.Products.Add(newProduct);

            var newProduct = Mapper.Map<ProductEntity>(product);
            _productRepository.AddProduct(newProduct);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品的时候出错");
            }

            var dto = Mapper.Map<ProductWithoutMaterialDto>(newProduct);

            // return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
            return CreatedAtRoute("GetProduct", new { id = dto.Id }, dto);
        }
        public IActionResult Put(int id, [FromBody] ProductModification productModification)
        {
            if (productModification == null)
            {
                return BadRequest();
            }

            if (productModification.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //  var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            var product = _productRepository.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            //model.Name = product.Name;
            //model.Price = product.Price;
            //model.Description = product.Description;
            Mapper.Map(productModification, product);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存产品的时候出错");
            }

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

            var productEntity = _productRepository.GetProduct(id);
            if (productEntity == null)
            {
                return NotFound();
            }

            var toPatch = Mapper.Map<ProductModification>(productEntity);
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
            Mapper.Map(toPatch, productEntity);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "更新的时候出错");
            }

            //model.Name = toPatch.Name;
            //model.Description = toPatch.Description;
            //model.Price = toPatch.Price;

            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            ////  var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            //  if (model == null)
            //  {
            //      return NotFound();
            //  }
            //  ProductService.Current.Products.Remove(model);
            var model = _productRepository.GetProduct(id);
            if (model == null)
            {
                return NotFound();
            }
            _productRepository.DeleteProduct(model);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "删除的时候出错");
            }
            _mailService.Send("Product Deleted", $"Id为{id}的产品被删除了");
            return NoContent();
        }
    }
}