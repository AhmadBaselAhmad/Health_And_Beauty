using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Dynamic_AttributeController : ControllerBase
    {
        private IDynamic_AttributeService _Dynamic_AttributeService;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public Dynamic_AttributeController(IDynamic_AttributeService Dynamic_AttributeService, IHttpContextAccessor HttpContextAccessor)
        {
            _Dynamic_AttributeService = Dynamic_AttributeService;
            _HttpContextAccessor = HttpContextAccessor;
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
            JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

            int ClinicId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
                .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[4].Value);

            ApiResponse? Response = _Dynamic_AttributeService.GetAllDynamicAttributes(Filter, ClinicId, OnlyHealthStandards);

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
        [HttpPost("AddNewDynamicAttribute")]
        public IActionResult AddNewDynamicAttribute(AddDynamic_AttributeViewModel NewDynamicAttribute)
        {
            JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

            NewDynamicAttribute.ClinicId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
                .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[4].Value);

            ApiResponse? Response = _Dynamic_AttributeService.AddNewDynamicAttribute(NewDynamicAttribute);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPatch("EditDynamicAttribute")]
        public IActionResult EditDynamicAttribute(EditDynamic_AttributeViewModel EditDynamicAttribute)
        {
            ApiResponse? Response = _Dynamic_AttributeService.EditDynamicAttribute(EditDynamicAttribute);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("ChangeDynamicAttributeRequiredStatus")]
        public IActionResult ChangeDynamicAttributeRequiredStatus(int DynmaicAttributeId)
        {
            ApiResponse? Response = _Dynamic_AttributeService.ChangeDynamicAttributeRequiredStatus(DynmaicAttributeId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("ChangeDynamicAttributeDisableStatus")]
        public IActionResult ChangeDynamicAttributeDisableStatus(int DynmaicAttributeId)
        {
            ApiResponse? Response = _Dynamic_AttributeService.ChangeDynamicAttributeDisableStatus(DynmaicAttributeId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("ChangeDynamicAttributeHealthStandardStatus")]
        public IActionResult ChangeDynamicAttributeHealthStandardStatus(int DynmaicAttributeId)
        {
            ApiResponse? Response = _Dynamic_AttributeService.ChangeDynamicAttributeHealthStandardStatus(DynmaicAttributeId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
