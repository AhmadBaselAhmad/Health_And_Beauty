using GraduationProject.DataBase.Helpers;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Working_DaysController : ControllerBase
    {
        private IWorking_DaysService _Working_DaysService;
        public Working_DaysController(IWorking_DaysService Working_DaysService)
        {
            _Working_DaysService = Working_DaysService;
        }
        [HttpPost("GetAllWorkingDays")]
        public IActionResult GetAllWorkingDays(ComplexFilter Filter)
        {
            ApiResponse Response = _Working_DaysService.GetAllWorkingDays(Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("UpdateWorking_DayStatus")]
        public IActionResult UpdateWorking_DayStatus(int Working_DayId)
        {
            ApiResponse Response = _Working_DaysService.UpdateWorking_DayStatus(Working_DayId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
