using GraduationProject.DataBase.Models;
using GraduationProject.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace GraduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalInformationController : ControllerBase
    {
        private IMedicalInformationService _MedicalInformationService;
        private IWebHostEnvironment _env;
        public MedicalInformationController(IMedicalInformationService MedicalInformationService, IWebHostEnvironment env)
        {
            _MedicalInformationService = MedicalInformationService;
            _env = env;
        }
        [HttpGet("GetAllAlergies")]
        public IActionResult GetAllAlergies()
        {
            string AllergiesFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Allergies.xlsx";

            using (var stream = System.IO.File.OpenRead(AllergiesFileRoot))
            {
                IFormFile AllergiesFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var response = _MedicalInformationService.GetAllAlergies(AllergiesFile);
                return Ok(response);
            }
        }
        [HttpGet("GetAllImmunizations")]
        public IActionResult GetAllImmunizations()
        {
            string ImmunizationsFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Immunizations.xlsx";

            using (var stream = System.IO.File.OpenRead(ImmunizationsFileRoot))
            {
                IFormFile ImmunizationsFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var response = _MedicalInformationService.GetAllImmunizations(ImmunizationsFile);
                return Ok(response);
            }
        }
        [HttpGet("GetAllMedicines")]
        public IActionResult GetAllMedicines()
        {
            string MedicinesFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Medicines.xlsx";

            using (var stream = System.IO.File.OpenRead(MedicinesFileRoot))
            {
                IFormFile MedicinesFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var response = _MedicalInformationService.GetAllMedicines(MedicinesFile);
                return Ok(response);
            }
        }
        [HttpGet("GetAllSurgeries")]
        public IActionResult GetAllSurgeries()
        {
            string SurgeriesFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Surgeries.xlsx";

            using (var stream = System.IO.File.OpenRead(SurgeriesFileRoot))
            {
                IFormFile SurgeriesFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var response = _MedicalInformationService.GetAllSurgeries(SurgeriesFile);
                return Ok(response);
            }
        }

    }
}
