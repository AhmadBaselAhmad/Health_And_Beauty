using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Allergies;
using GraduationProject.DataBase.ViewModels.Immunization;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Surgery;
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

                var Response = _MedicalInformationService.GetAllAlergies(AllergiesFile);
                return Ok(Response);
            }
        }
        [HttpGet("GetAllImmunizations")]
        public IActionResult GetAllImmunizations()
        {
            string ImmunizationsFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Immunizations.xlsx";

            using (var stream = System.IO.File.OpenRead(ImmunizationsFileRoot))
            {
                IFormFile ImmunizationsFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var Response = _MedicalInformationService.GetAllImmunizations(ImmunizationsFile);
                return Ok(Response);
            }
        }
        [HttpGet("GetAllMedicines")]
        public IActionResult GetAllMedicines()
        {
            string MedicinesFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Medicines.xlsx";

            using (var stream = System.IO.File.OpenRead(MedicinesFileRoot))
            {
                IFormFile MedicinesFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var Response = _MedicalInformationService.GetAllMedicines(MedicinesFile);
                return Ok(Response);
            }
        }
        [HttpGet("GetAllSurgeries")]
        public IActionResult GetAllSurgeries()
        {
            string SurgeriesFileRoot = _env.ContentRootPath + "MedicalFiles\\" + "Surgeries.xlsx";

            using (var stream = System.IO.File.OpenRead(SurgeriesFileRoot))
            {
                IFormFile SurgeriesFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));

                var Response = _MedicalInformationService.GetAllSurgeries(SurgeriesFile);
                return Ok(Response);
            }
        }
        [HttpPost("AddAllergies")]
        public IActionResult AddAllergies(List<AddAllergyViewModel> NewAllergies)
        {
            var Response = _MedicalInformationService.AddAllergies(NewAllergies);
            return Ok(Response);
        }
        [HttpPost("AddImmunizations")]
        public IActionResult AddImmunizations(List<AddImmunizationViewModel> NewImmunizations)
        {
            var Response = _MedicalInformationService.AddImmunizations(NewImmunizations);
            return Ok(Response);
        }
        [HttpPost("AddMedicines")]
        public IActionResult AddMedicines(List<AddMedicineViewModel> NewMedicines)
        {
            var Response = _MedicalInformationService.AddMedicines(NewMedicines);
            return Ok(Response);
        }
        [HttpPost("AddSurgeries")]
        public IActionResult AddSurgeries(List<AddSurgeryViewModel> NewSurgeries)
        {
            var Response = _MedicalInformationService.AddSurgeries(NewSurgeries);
            return Ok(Response);
        }
    }
}
