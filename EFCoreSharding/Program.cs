﻿using System;
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
            services.AddDbContext<MyDbContext>(DIExtension.UseDefaultSharding<MyDbContext>);
            //额外添加分片配置
            services.AddShardingConfigure<MyDbContext>()
                .AddEntityConfig(op =>
                {
                    op.CreateShardingTableOnStart = true;
                    op.EnsureCreatedWithOutShardingTable = true;
                    op.UseShardingQuery((conn, builder) =>
                    {
                        builder.UseSqlServer(conn);
                    });
                    op.UseShardingTransaction((conn, builder) =>
                    {
                        builder.UseSqlServer(conn);
                    });
                    op.AddShardingTableRoute<OrderVirtualTableRoute>();
                }).AddConfig(op =>
                {
                    op.ConfigId = "c1";
                    op.AddDefaultDataSource(Guid.NewGuid().ToString("n"),
                        "Data Source=localhost;Initial Catalog=EFCoreShardingDB;Integrated Security=True;");
                }).EnsureConfig();

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
