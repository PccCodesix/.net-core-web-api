using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entity;

namespace WebApi.Repositories
{
   public interface IProductRepository
    {
        IEnumerable<ProductEntity> GetProducts();
        ProductEntity GetProduct(int productId, bool includeMaterials = false);
        IEnumerable<MaterialEntity> GetMaterialsForProduct(int productId);
        MaterialEntity GetMaterialForProduct(int productId, int materialId);
        bool ProductExist(int productId);
        void AddProduct(ProductEntity product);
        bool Save();
        void DeleteProduct(ProductEntity product);

    }
}
