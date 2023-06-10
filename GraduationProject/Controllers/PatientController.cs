using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IPatientService _PatientService;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public PatientController(IPatientService PatientService, IHttpContextAccessor HttpContextAccessor)
        {
            _PatientService = PatientService;
            _HttpContextAccessor = HttpContextAccessor;
        }
        [HttpPost("AddPatientMedicalInfo")]
        public IActionResult AddPatientMedicalInfo(AddMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            ApiResponse? Response = _PatientService.AddPatientMedicalInfo(PatientMedicalInformationViewModel);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPatch("EditPatientMedicalInfo")]
        public IActionResult EditPatientMedicalInfo(EditMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            ApiResponse? Response = _PatientService.EditPatientMedicalInfo(PatientMedicalInformationViewModel);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetMedicalInformationByPatientId")]
        public IActionResult GetMedicalInformationByPatientId(int PatientId)
        {
            ApiResponse? Response = _PatientService.GetMedicalInformationByPatientId(PatientId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetPatientById")]
        public IActionResult GetPatientById(int PatientId)
        {
            ApiResponse? Response = _PatientService.GetPatientById(PatientId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllDocotorsPatients")]
        public IActionResult GetAllDocotorsPatients(ComplexFilter Filter)
        {
            JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

            int UserId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
                .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[0].Value);

            ApiResponse? Response = _PatientService.GetAllDoctorsPatients(UserId, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
