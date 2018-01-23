using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Entity;

namespace WebApi.Repositories
{
    public class ProductRepository : IProductRepository


    {
        /// <summary>
        /// GetMaterialsForProduct，查询某个产品下所有的原料。
        //GetMaterialForProduct，查询某个产品下的某种原料。
        /// </summary>
        private readonly MyContext _myContext;

        public ProductRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public void AddProduct(ProductEntity product)
        {
            _myContext.ProductEntitys.Add(product);

        }



        public void DeleteProduct(ProductEntity product)
        {
            _myContext.ProductEntitys.Remove(product);

        }

        public MaterialEntity GetMaterialForProduct(int productId, int materialId)
        {
            return _myContext.MaterialEntitys.FirstOrDefault(x => x.ProductId == productId && x.Id == materialId);

        }

        public IEnumerable<MaterialEntity> GetMaterialsForProduct(int productId)
        {
            return _myContext.MaterialEntitys.Where(x => x.ProductId == productId).ToList();

        }

        public ProductEntity GetProduct(int productId, bool includeMaterials)
        {
            if (includeMaterials)
            {
                return _myContext.ProductEntitys
                    .Include(x => x.Materials).FirstOrDefault(x => x.Id == productId);
            }
            return _myContext.ProductEntitys.Find(productId);

        }

        public IEnumerable<ProductEntity> GetProducts()
        {
            return _myContext.ProductEntitys.OrderBy(x => x.Name).ToList();

        }

        public bool ProductExist(int productId)
        {
            return _myContext.ProductEntitys.Any(x => x.Id == productId);

        }

        public bool Save()
        {
            return _myContext.SaveChanges() >= 0;

        }
    }
}
