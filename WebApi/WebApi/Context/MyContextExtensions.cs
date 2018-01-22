using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entity;

namespace WebApi.Context
{
    public static class MyContextExtensions
    {
        public static void EnsureSeedDataForContext(this MyContext context)
        {
            if (context.ProductEntitys.Any())
            {
                return;
            }
            var products = new List<ProductEntity>
            {
                new ProductEntity
                {
                    Name = "牛奶",
                    Price = 2.5f,
                    Description = "这是牛奶啊"
                },
                new ProductEntity
                {
                    Name = "面包",
                    Price = 4.5f,
                    Description = "这是面包啊"
                },
                new ProductEntity
                {
                    Name = "啤酒",
                    Price = 7.5f,
                    Description = "这是啤酒啊"
                }
            };
            context.ProductEntitys.AddRange(products);
            context.SaveChanges();
        }

    }
}
