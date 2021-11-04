﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreShardingTable.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ShardingCore.Bootstrapers;

namespace EFCoreShardingTable
{
    public static class StartupExtension
    {
        public static void UseShardingCore(this IApplicationBuilder app)
        {
            app.ApplicationServices.GetRequiredService<IShardingBootstrapper>().Start();
        }
        public static void InitSeed(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var myDbContext = serviceScope.ServiceProvider.GetRequiredService<MyDbContext>();
                if (!myDbContext.Set<Setting>().Any())
                {
                    List<Setting> settings = new List<Setting>(3);
                    settings.Add(new Setting()
                    {
                        Code = "Admin",
                        Name = "AdminName"
                    });
                    settings.Add(new Setting()
                    {
                        Code = "User",
                        Name = "UserName"
                    });
                    settings.Add(new Setting()
                    {
                        Code = "SuperAdmin",
                        Name = "SuperAdminName"
                    });
                    string[] areas = new string[] {"A","B","C" };
                    List<SysUser> users = new List<SysUser>(10);
                    for (int i = 0; i < 10; i++)
                    {
                        var uer=new SysUser()
                        {
                            Id = i.ToString(),
                            Name = $"MyName{i}",
                            SettingCode = settings[i % 3].Code,
                            Area = areas[i % 3]
                        };
                        users.Add(uer);
                    }
                    List<Order> orders = new List<Order>(300);
                    var begin = new DateTime(2021, 1, 1, 3, 3, 3);
                    for (int i = 0; i < 300; i++)
                    {

                        var order = new Order()
                        {
                            Id = i.ToString(),
                            Payer = $"{i % 10}",
                            Money = 100+new Random().Next(100,3000),
                            OrderStatus = (OrderStatusEnum)(i % 4 + 1),
                            Area = areas[i % 3],
                            CreationTime = begin.AddDays(i)
                        };
                        orders.Add(order);
                    }
                    myDbContext.AddRange(settings);
                    myDbContext.AddRange(users);
                    myDbContext.AddRange(orders);
                    myDbContext.SaveChanges();
                }
            }
        }
    }
}
