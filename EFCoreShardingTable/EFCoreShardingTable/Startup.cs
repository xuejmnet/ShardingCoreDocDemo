using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreShardingTable.VirtualRoutes;
using Microsoft.EntityFrameworkCore;
using ShardingCore;
using ShardingCore.Bootstrapers;

namespace EFCoreShardingTable
{
    public class Startup
    {
        public static readonly ILoggerFactory efLogger = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information).AddConsole();
        });
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            //∂ÓÕ‚ÃÌº”∑÷∆¨≈‰÷√
            services.AddShardingDbContext<MyDbContext>()
                .AddEntityConfig(op =>
                {
                    op.CreateShardingTableOnStart = true;
                    op.EnsureCreatedWithOutShardingTable = true;
                    op.UseShardingQuery((conn, builder) =>
                    {
                        builder.UseSqlServer(conn).UseLoggerFactory(efLogger);
                    });
                    op.UseShardingTransaction((conn, builder) =>
                    {
                        builder.UseSqlServer(conn).UseLoggerFactory(efLogger);
                    });
                    op.AddShardingTableRoute<SysUserVirtualTableRoute>();
                    op.AddShardingTableRoute<OrderVirtualTableRoute>();
                }).AddConfig(op =>
                {
                    op.ConfigId = "c1";
                    op.AddDefaultDataSource(Guid.NewGuid().ToString("n"),
                        "Data Source=localhost;Initial Catalog=EFCoreShardingTableDB;Integrated Security=True;");
                }).EnsureConfig();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseShardingCore();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.InitSeed();
        }
    }
}
