using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Authenticate;
using GraduationProject.DataBase.ViewModels.Doctor;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private IDoctorService _DoctorService;
        public DoctorController(IDoctorService DoctorService)
        {
            _DoctorService = DoctorService;
        }
        [HttpPost("AddNewDoctor")]
        public IActionResult AddNewDoctor(AddDoctorViewModel NewDoctor)
        {
            ApiResponse? Response = _DoctorService.AddNewDoctor(NewDoctor);
            return Ok(Response);
        }
    }
}
