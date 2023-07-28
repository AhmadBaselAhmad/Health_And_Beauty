using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Secretary;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretaryController : ControllerBase
    {
        private ISecretaryService _SecretaryService;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        public SecretaryController(ISecretaryService SecretaryService, IHttpContextAccessor HttpContextAccessor)
        {
            _SecretaryService = SecretaryService;
            _HttpContextAccessor = HttpContextAccessor;
        }
        [HttpGet("GetSecretaryById")]
        public IActionResult GetSecretaryById(int SecretaryId)
        {
            ApiResponse Response = _SecretaryService.GetSecretaryById(SecretaryId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPut("DeleteSecreatry")]
        public IActionResult DeleteSecreatry(int SecretaryId)
        {
            ApiResponse? Response = _SecretaryService.DeleteSecreatry(SecretaryId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("AddNewSecretary")]
        public IActionResult AddNewSecretary(AddSecreataryViewModel NewSecretary)
        {
            ApiResponse? Response = _SecretaryService.AddNewSecretary(NewSecretary);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPatch("EditClinicSecretary")]
        public IActionResult EditClinicSecretary(int SecretaryId, int NewClinicId)
        {
            ApiResponse? Response = _SecretaryService.EditClinicSecretary(SecretaryId, NewClinicId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllSecretaries")]
        public IActionResult GetAllSecretaries(int ClinicId, ComplexFilter Filter)
        {
            ApiResponse? Response = _SecretaryService.GetAllSecretaries(ClinicId, Filter);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("GetAllDoctorsByClinicId")]
        public IActionResult GetAllDoctorsByClinicId()
        {
            JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

            int ClinicId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
                .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[4].Value);

            ApiResponse? Response = _SecretaryService.GetAllDoctorsByClinicId(ClinicId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditSecretary")]
        public IActionResult EditSecretary(EditSecretaryViewModel SecretaryNewData)
        {
            ApiResponse? Response = _SecretaryService.EditSecretary(SecretaryNewData);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
