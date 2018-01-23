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
                    Price = new decimal(2.5),
                    Description = "这是牛奶啊",
                    Materials = new List<MaterialEntity>
                    {
                        new MaterialEntity
                        {
                            Name = "水"
                        },
                        new MaterialEntity
                        {
                            Name = "奶粉"
                        }
                    }
                },
                new ProductEntity
                {
                    Name = "面包",
                    Price = new decimal(4.5),
                    Description = "这是面包啊",
                    Materials = new List<MaterialEntity>
                    {
                        new MaterialEntity
                        {
                            Name = "面粉"
                        },
                        new MaterialEntity
                        {
                            Name = "糖"
                        }
                    }
                },
                new ProductEntity
                {
                    Name = "啤酒",
                    Price = new decimal(7.5),
                    Description = "这是啤酒啊",
                    Materials = new List<MaterialEntity>
                    {
                        new MaterialEntity
                        {
                            Name = "麦芽"
                        },
                        new MaterialEntity
                        {
                            Name = "地下水"
                        }
                    }
                }
            };
            context.ProductEntitys.AddRange(products);
            context.SaveChanges();
        }

    }
}
