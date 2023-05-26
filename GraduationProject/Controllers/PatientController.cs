using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.Medical_Information;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private IPatientService _PatientService;
        public PatientController(IPatientService PatientService)
        {
            _PatientService = PatientService;
        }
        [HttpPost("AddPatientMedicalInfo")]
        public IActionResult AddPatientMedicalInfo(AddMedical_InformationsViewModel PatientMedicalInformationViewModel)
        {
            ApiResponse? Response = _PatientService.AddPatientMedicalInfo(PatientMedicalInformationViewModel);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditPatientMedicalInfo")]
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
            int UserId = 0;
            ApiResponse? Response = _PatientService.GetAllDocotorsPatients(UserId, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
