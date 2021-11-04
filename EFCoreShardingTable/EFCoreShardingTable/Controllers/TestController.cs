﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreShardingTable.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreShardingTable.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext _myDbContext;

        public TestController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }
        public async Task<IActionResult> Query()
        {
            var sysUser =await _myDbContext.Set<SysUser>().Where(o=>o.Id=="1").FirstOrDefaultAsync();
            var dateTime = new DateTime(2021,3,5);
            var order = await _myDbContext.Set<Order>().Where(o=>o.CreationTime>= dateTime).OrderBy(o=>o.CreationTime).FirstOrDefaultAsync();
            var orderIdOne = await _myDbContext.Set<Order>().FirstOrDefaultAsync(o => o.Id == "3");


            var sysUsers = await _myDbContext.Set<SysUser>().Where(o => o.Id == "1" || o.Id=="6").ToListAsync();

            return Ok(new object[]
            {
                sysUser,
                order,
                orderIdOne,
                sysUsers
            });
        }
        public async Task<IActionResult> QueryJoin1()
        {
           var sql= from user in _myDbContext.Set<SysUser>().Where(o => o.Id == "1" || o.Id == "6")
                join setting in _myDbContext.Set<Setting>()
                    on user.SettingCode equals setting.Code
                select new
                {
                    user.Id,
                    user.Name,
                    user.Area,
                    user.SettingCode,
                    SettingName=setting.Name,
                };
            return Ok(await sql.ToListAsync());
        }
        public async Task<IActionResult> QueryJoin2()
        {
           var begin = new DateTime(2021, 3, 2);
           var end = new DateTime(2021, 4, 3);
           var sql1 = from user in _myDbContext.Set<SysUser>().Where(o => o.Id == "1" || o.Id == "6")
               join order in _myDbContext.Set<Order>().Where(o=>o.CreationTime>=begin&&o.CreationTime<=end)
                   on user.Id equals order.Payer
               select new
               {
                   user.Id,
                   user.Name,
                   user.Area,
                   user.SettingCode,
                   OrderId = order.Id,
                   order.Payer,
                   order.CreationTime
               };
            return Ok(await sql1.ToListAsync());
        }
        public async Task<IActionResult> Update()
        {
            var sysUser = await _myDbContext.Set<SysUser>().Where(o => o.Id == "1").FirstOrDefaultAsync();
            sysUser.Name = "new name";
            var i=await _myDbContext.SaveChangesAsync();
            return Ok(i);
        }
        public async Task<IActionResult> Update1()
        {
            var sysUser = await _myDbContext.Set<SysUser>().AsNoTracking().Where(o => o.Id == "1").FirstOrDefaultAsync();
            sysUser.Name = "new name";
            _myDbContext.Update(sysUser);
            var i = await _myDbContext.SaveChangesAsync();
            return Ok(i);
        }
    }
}