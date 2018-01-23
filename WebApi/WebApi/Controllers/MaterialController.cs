using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Controllers
{
    //[Produces("application/json")]
    [Route("api/product")]
    public class MaterialController : Controller
    {

        private readonly IProductRepository _productRepository;
        public MaterialController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("{productId}/materials")]
        public IActionResult GetMaterials(int productId)
        {
            //var product = ProductService.Current.Products.SingleOrDefault(x => x.Id == productId);
            //if (product == null)
            //{
            //    return NotFound();
            //}
            //return Ok(product.Materials);
            var product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }

            var materials = _productRepository.GetMaterialsForProduct(productId);
            var results = materials.Select(material => new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            })
                .ToList();
            return Ok(results);

        }

        [HttpGet("{productId}/materials/{id}")]
        public IActionResult GetMaterial(int productId, int id)
        {
            //var product = ProductService.Current.Products.SingleOrDefault(x => x.Id == productId);
            //if (product == null)
            //{
            //    return NotFound();
            //}
            //var material = product.Materials.SingleOrDefault(x => x.Id == id);
            //if (material == null)
            //{
            //    return NotFound();
            //}
            //return Ok(material);
            var product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }

            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }
            var result = new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            };
            return Ok(result);
            //product 由materia组成
            // productid 下 材料id 为 id 的材料
        }

    }
}