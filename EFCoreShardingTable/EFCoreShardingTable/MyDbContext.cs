﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreShardingTable.Entities;
using Microsoft.EntityFrameworkCore;
using ShardingCore.Core.VirtualRoutes.TableRoutes.RouteTails.Abstractions;
using ShardingCore.Sharding;
using ShardingCore.Sharding.Abstractions;

namespace EFCoreShardingTable
{
    public class MyDbContext:AbstractShardingDbContext,IShardingTableDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o=>o.Payer).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o => o.Area).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o => o.OrderStatus).HasConversion<int>();
                entity.ToTable(nameof(Order));
            });
            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Id).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o=>o.Name).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o=>o.Area).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o => o.SettingCode).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.ToTable(nameof(SysUser));
            });
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(o => o.Code);
                entity.Property(o => o.Code).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.Property(o=>o.Name).IsRequired().IsUnicode(false).HasMaxLength(50);
                entity.ToTable(nameof(Setting));
            });
        }

        public IRouteTail RouteTail { get; set; }
    }
}
