using GraduationProject.DataBase.Helpers;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicController : ControllerBase
    {
        private IClinicService _ClinicService;
        public ClinicController(IClinicService ClinicService)
        {
            _ClinicService = ClinicService;
        }
        [HttpPost("GetAllClinics")]
        public IActionResult GetAllClinics(ComplexFilter Filter)
        {
            ApiResponse Response = _ClinicService.GetAllClinics(Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
