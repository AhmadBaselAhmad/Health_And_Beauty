using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Service;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyClinicServiceController : ControllerBase
    {
        private IMyClinicService _ClinicService;
        public MyClinicServiceController(IMyClinicService ClinicService)
        {
            _ClinicService = ClinicService;
        }
        [HttpPost("GetAllClinicServices")]
        public IActionResult GetAllClinicServices(ComplexFilter Filter, int? ClinicId)
        {
            ApiResponse Response = _ClinicService.GetAllClinicServices(Filter, ClinicId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("AddNewClinicService")]
        public IActionResult AddNewClinicService(AddClinicServiceViewModel AddClinicServiceViewModel)
        {
            ApiResponse Response = _ClinicService.AddNewClinicService(AddClinicServiceViewModel);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetClinicServiceById")]
        public IActionResult GetClinicServiceById(int ClinicServiceId)
        {
            ApiResponse Response = _ClinicService.GetClinicServiceById(ClinicServiceId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPatch("EditClinicService")]
        public IActionResult EditClinicService(EditClinicServiceViewModel EditClinicServiceViewModel)
        {
            ApiResponse Response = _ClinicService.EditClinicService(EditClinicServiceViewModel);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("DeleteClinicService")]
        public IActionResult DeleteClinicService(int ClinicServiceId)
        {
            ApiResponse Response = _ClinicService.DeleteClinicService(ClinicServiceId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
