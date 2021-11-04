using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore;
using ShardingCore.Bootstrapers;

namespace EFCoreSharding
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            //原来的dbcontext配置
            services.AddDbContext<MyDbContext>(o =>
                o.UseSqlServer("Data Source=localhost;Initial Catalog=EFCoreShardingDB;Integrated Security=True;")
                    .UseSharding<MyDbContext>()//需要添加
                );
            //额外添加分片配置
            services.AddShardingConfigure<MyDbContext>((conn, builder) =>
                {
                    builder.UseSqlServer(conn);
                }).Begin(o =>
                {
                    o.AutoTrackEntity = true;
                    o.CreateShardingTableOnStart = true;
                    o.EnsureCreatedWithOutShardingTable = true;
                }).AddShardingTransaction((connection, builder) =>
                {
                    builder.UseSqlServer(connection);
                }).AddDefaultDataSource(Guid.NewGuid().ToString("n"),
                    "Data Source=localhost;Initial Catalog=EFCoreShardingDB;Integrated Security=True;")
                .AddShardingTableRoute(op => { op.AddShardingTableRoute<OrderVirtualTableRoute>(); }).End();

            var buildServiceProvider = services.BuildServiceProvider();
            //启动必备
            buildServiceProvider.GetRequiredService<IShardingBootstrapper>().Start();
            using (var scope = buildServiceProvider.CreateScope())
            {
                var myDbContext = scope.ServiceProvider.GetService<MyDbContext>();
                //如果不存在就创建数据库和对应的数据库表
                //myDbContext.Database.EnsureCreated();

                var now = new DateTime(2021,1,1);
                var orders = Enumerable.Range(0,10).Select((o,i)=>new Order()
                {
                    Id = i.ToString(),
                    CreateTime = now.AddDays(i),
                    Payer = $"用户:{i}",
                    Money = i*100,
                    IsDelete = false
                }).ToList();
                myDbContext.AddRange(orders);
                myDbContext.SaveChanges();
            }
        }
    }
}
