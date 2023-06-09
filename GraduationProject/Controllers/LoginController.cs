﻿using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Authenticate;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginService _LoginService;
        public LoginController(ILoginService LoginService)
        {
            _LoginService = LoginService;
        }
        [HttpPost("CreateToken")]
        public IActionResult CreateToken(LoginViewModel Login)
        {
            ApiResponse? Response = _LoginService.CreateToken(Login);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
