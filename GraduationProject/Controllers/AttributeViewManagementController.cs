using GraduationProject.DataBase.Helpers;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttributeViewManagementController : ControllerBase
    {
        private IAttributeViewManagement _AttributeViewManagement;
        public AttributeViewManagementController(IAttributeViewManagement AttributeViewManagement)
        {
            _AttributeViewManagement = AttributeViewManagement;
        }
        [HttpPost("GetAllAttributeViewManagement")]
        public IActionResult GetAllAttributeViewManagement(int ClinicId, ComplexFilter Filter)
        {
            ApiResponse Response = _AttributeViewManagement.GetAllAttributeViewManagement(ClinicId, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("UpdateAttributeStatus")]
        public IActionResult UpdateAttributeStatus(int AttributeViewManagementId)
        {
            ApiResponse Response = _AttributeViewManagement.UpdateAttributeStatus(AttributeViewManagementId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
