using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Authenticate;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private IDoctorService _DoctorService;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public DoctorController(IDoctorService DoctorService, IHttpContextAccessor HttpContextAccessor)
        {
            _DoctorService = DoctorService;
            _HttpContextAccessor = HttpContextAccessor;
        }
        [HttpPost("AddNewDoctor")]
        public IActionResult AddNewDoctor(AddDoctorViewModel NewDoctor)
        {
            ApiResponse? Response = _DoctorService.AddNewDoctor(NewDoctor);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllDoctors")]
        public IActionResult GetAllDoctors(ComplexFilter Filter, int? ClinicId)
        {
            ApiResponse? Response = _DoctorService.GetAllDoctors(Filter, ClinicId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("EditDoctor")]
        public IActionResult EditDoctor(EditDoctorViewModel DoctorNewData)
        {
            ApiResponse? Response = _DoctorService.EditDoctor(DoctorNewData);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetDoctorById")]
        public IActionResult GetDoctorById(int DoctorId)
        {
            ApiResponse? Response = _DoctorService.GetDoctorById(DoctorId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("DeleteDoctor")]
        public IActionResult DeleteDoctor(int DoctorId)
        {
            ApiResponse? Response = _DoctorService.DeleteDoctor(DoctorId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
