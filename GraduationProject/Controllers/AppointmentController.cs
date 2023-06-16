using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private IAppointmentService _AppointmentService;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public AppointmentController(IAppointmentService AppointmentService, IHttpContextAccessor HttpContextAccessor)
        {
            _AppointmentService = AppointmentService;
            _HttpContextAccessor = HttpContextAccessor;
        }
        [HttpPost("GetAllAppointmentsForDoctorRole")]
        public IActionResult GetAllAppointmentsForDoctorRole(string AppointmentStatus, ComplexFilter Filter)
        {
            JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

            int UserId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
                .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[0].Value);

            ApiResponse Response = _AppointmentService.GetAllAppointmentsForDoctorRole(UserId, AppointmentStatus, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllAppointmentsForSecretaryRole")]
        public IActionResult GetAllAppointmentsForSecretaryRole(int DoctorId, string AppointmentStatus, ComplexFilter Filter)
        {
            ApiResponse Response = _AppointmentService.GetAllAppointmentsForSecretaryRole(DoctorId, AppointmentStatus, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
