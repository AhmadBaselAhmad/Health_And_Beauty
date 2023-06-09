﻿using GraduationProject.DataBase.Context;
using GraduationProject.DataBase.Helpers;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdminService _AdminService;
        public AdminController(IAdminService AdminService)
        {
            _AdminService = AdminService;
        }

        [HttpPut("ChangeCurrentAdmin")]
        public IActionResult ChangeCurrentAdmin(int DoctorId)
        {
            ApiResponse Response = _AdminService.ChangeCurrentAdmin(DoctorId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok();
        }
    }
}
