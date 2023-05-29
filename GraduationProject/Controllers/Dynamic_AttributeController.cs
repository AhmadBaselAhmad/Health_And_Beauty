using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Dynamic_AttributeController : ControllerBase
    {
        private IDynamic_AttributeService _Dynamic_AttributeService;
        public Dynamic_AttributeController(IDynamic_AttributeService Dynamic_AttributeService)
        {
            _Dynamic_AttributeService = Dynamic_AttributeService;
        }
        [HttpGet("GetDynamicAttributeById")]
        public IActionResult GetDynamicAttributeById(int Dynamic_AttributeId)
        {
            ApiResponse? Response = _Dynamic_AttributeService.GetDynamicAttributeById(Dynamic_AttributeId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllDynamicAttributes")]
        public IActionResult GetAllDynamicAttributes(ComplexFilter Filter, bool? OnlyHealthStandards)
        {
            ApiResponse? Response = _Dynamic_AttributeService.GetAllDynamicAttributes(Filter, OnlyHealthStandards);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetAllOperations")]
        public IActionResult GetAllOperations()
        {
            ApiResponse? Response = _Dynamic_AttributeService.GetAllOperations();

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetAllDataTypes")]
        public IActionResult GetAllDataTypes()
        {
            ApiResponse? Response = _Dynamic_AttributeService.GetAllDataTypes();

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
