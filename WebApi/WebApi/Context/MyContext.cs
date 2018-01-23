using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Entity;

namespace WebApi.Context
{
    public  class MyContext: DbContext
    {
        public DbSet<ProductEntity> ProductEntitys { get; set; }
        public DbSet<MaterialEntity> MaterialEntitys { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;database=TestDb2;userid='root';pwd='p123456';SslMode=none");
            base.OnConfiguring(optionsBuilder);
        }
        public MyContext(DbContextOptions<MyContext> options)
           : base(options)
        {
            // Database.EnsureCreated();
            Database.Migrate();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //简化配置 对应的每个ENTITy 都有自己的配置
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            //modelBuilder.Entity<ProductEntity>().HasKey(x => x.Id);
            //modelBuilder.Entity<ProductEntity>().Property(x => x.Name).IsRequired().HasMaxLength(50);
            //modelBuilder.Entity<ProductEntity>().Property(x => x.Price).HasColumnType("decimal(8,2)");
        }

    }
}
