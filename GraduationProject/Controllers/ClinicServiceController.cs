using GraduationProject.DataBase.Helpers;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicServiceController : ControllerBase
    {
        private IMyClinicService _ClinicService;
        public ClinicServiceController(IMyClinicService ClinicService)
        {
            _ClinicService = ClinicService;
        }
        [HttpPost("GetAllClinicServices")]
        public IActionResult GetAllClinicServices(int ClinicId, ComplexFilter Filter)
        {
            int UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.Items["UserId"]);

            ApiResponse Response = _ClinicService.GetAllClinicServices(ClinicId, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
