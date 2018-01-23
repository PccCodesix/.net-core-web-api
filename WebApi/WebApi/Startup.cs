using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WebApi.Context;
using WebApi.Dto;
using WebApi.Entity;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {

        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                            //.AddJsonOptions(options=> {
                            //    if (options.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
                            //    {
                            //        resolver.NamingStrategy = null;
                            //    }
                            //asp.net core 2.0 默认返回的结果格式是Json, 并使用json.net对结果默认做了camel case的转化(大概可理解为首字母小写). 
                            //这一点与老.net web api 不一样, 原来的 asp.net web api 默认不适用任何NamingStrategy, 需要手动加上camelcase的转化.
                            //我很喜欢这样, 因为大多数前台框架例如angular等都约定使用camel case.
                            //如果非得把这个规则去掉, 那么就在configureServices里面改一下:
                            //});

                            //设置header为xml后,返回的还是json, 这是因为asp.net core 默认只实现了json.
                            //可以在ConfigureServices里面修改Mvc的配置来添加xml格式:
                            .AddMvcOptions(options =>
                            {
                                options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                            });

            ;
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            services.AddDbContext<MyContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MyContext mycontext)//
        {
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            mycontext.EnsureSeedDataForContext();
            app.UseStatusCodePages();// status code middleware like  (http code)
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ProductEntity, ProductWithoutMaterialDto>();
                cfg.CreateMap<ProductEntity, ProductDto>();
                cfg.CreateMap<ProductEntity, MaterialDto>();
                cfg.CreateMap<ProductModification, ProductEntity>();

            });


            app.UseMvc();

        }
    }
}
