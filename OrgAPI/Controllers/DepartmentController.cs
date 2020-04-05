﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgDAL;

namespace OrgAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        OrganizationDbContext _dbContext;
        UserManager<IdentityUser> userManager;
        public DepartmentController(OrganizationDbContext dbContext, UserManager<IdentityUser> _userManager)
        {
            _dbContext = dbContext;
            userManager = _userManager;
        }

        [HttpGet("ALLDepartments")]

        public IActionResult ALLDepartments()
        {
            var data = _dbContext.Departments.ToList();
            //int b = 0;
            //int a = 5 / b;
            return Ok(data);
        }

        [HttpPost]
        public IActionResult PostDepartment(Object a)
        {
            var claims = User.Claims.ToList();
            //getting name from the jwt token
            var name = User.Claims.Where(x => x.Type.Contains("claims/name")).FirstOrDefault().Value;
            var id = User.Claims.Where(x => x.Type.Contains("claims/nameidentifier")).FirstOrDefault().Value;
            return StatusCode(201);
        }

        [HttpPut("UpdateDepartment")]

        public IActionResult UpdateDepartment(Department dep)
        {
            var Dept = _dbContext.Departments.Where(x=>x.Did == dep.Did).AsNoTracking().FirstOrDefault();
            if(Dept != null)
            {
                _dbContext.Update(dep);
                _dbContext.SaveChanges();
            }
            else
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpDelete("DeleteDepartment/{id}")]
        
        public IActionResult DeleteDepartment(int id)
        {
            var Dept = _dbContext.Departments.Where(x => x.Did == id).AsNoTracking().FirstOrDefault();
            if(Dept !=null)
            {
                _dbContext.Remove(Dept);
                _dbContext.SaveChanges();
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }
    }
}