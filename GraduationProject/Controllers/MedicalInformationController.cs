using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Allergies;
using GraduationProject.DataBase.ViewModels.Immunization;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Surgery;
using GraduationProject.Service.Interfaces;
using GraduationProject.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using static System.Net.WebRequestMethods;

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

                ApiResponse Response = _MedicalInformationService.GetAllAlergies(AllergiesFile);

                if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                    return BadRequest(Response);

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

                ApiResponse Response = _MedicalInformationService.GetAllImmunizations(ImmunizationsFile);

                if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                    return BadRequest(Response);

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

                ApiResponse Response = _MedicalInformationService.GetAllMedicines(MedicinesFile);

                if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                    return BadRequest(Response);

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

                ApiResponse Response = _MedicalInformationService.GetAllSurgeries(SurgeriesFile);

                if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                    return BadRequest(Response);

                return Ok(Response);
            }
        }
        [HttpPost("AddAllergies")]
        public IActionResult AddAllergies(List<AddAllergyViewModel> NewAllergies)
        {
            ApiResponse Response = _MedicalInformationService.AddAllergies(NewAllergies);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("AddImmunizations")]
        public IActionResult AddImmunizations(List<AddImmunizationViewModel> NewImmunizations)
        {
            ApiResponse Response = _MedicalInformationService.AddImmunizations(NewImmunizations);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("AddMedicines")]
        public IActionResult AddMedicines(List<AddMedicineViewModel> NewMedicines)
        {
            ApiResponse Response = _MedicalInformationService.AddMedicines(NewMedicines);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("AddSurgeries")]
        public IActionResult AddSurgeries(List<AddSurgeryViewModel> NewSurgeries)
        {
            ApiResponse Response = _MedicalInformationService.AddSurgeries(NewSurgeries);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditAllergies")]
        public IActionResult EditAllergies(int MedicalInfoId, List<AddAllergyViewModel> NewAllergies)
        {
            ApiResponse Response = _MedicalInformationService.EditAllergies(MedicalInfoId, NewAllergies);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditImmunizations")]
        public IActionResult EditImmunizations(int MedicalInfoId, List<AddImmunizationViewModel> NewImmunizations)
        {
            ApiResponse Response = _MedicalInformationService.EditImmunizations(MedicalInfoId, NewImmunizations);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditMedicines")]
        public IActionResult EditMedicines(int MedicalInfoId, List<AddMedicineViewModel> NewMedicines)
        {
            ApiResponse Response = _MedicalInformationService.EditMedicines(MedicalInfoId, NewMedicines);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpPost("EditSurgeries")]
        public IActionResult EditSurgeries(int MedicalInfoId, List<AddSurgeryViewModel> NewSurgeries)
        {
            ApiResponse Response = _MedicalInformationService.EditSurgeries(MedicalInfoId, NewSurgeries);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetOldAllergies")]
        public IActionResult GetOldAllergies(int MedicalInfoId)
        {
            ApiResponse Response = _MedicalInformationService.GetOldAllergies(MedicalInfoId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetOldImmunizations")]
        public IActionResult GetOldImmunizations(int MedicalInfoId)
        {
            ApiResponse Response = _MedicalInformationService.GetOldImmunizations(MedicalInfoId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetOldMedicines")]
        public IActionResult GetOldMedicines(int MedicalInfoId)
        {
            ApiResponse Response = _MedicalInformationService.GetOldMedicines(MedicalInfoId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
        [HttpGet("GetOldSurgeries")]
        public IActionResult GetOldSurgeries(int MedicalInfoId)
        {
            ApiResponse Response = _MedicalInformationService.GetOldSurgeries(MedicalInfoId);

            if (!string.IsNullOrEmpty(Response.ErrorMessage) ? Response.ErrorMessage != "Succeed" : false)
                return BadRequest(Response);

            return Ok(Response);
        }
    }
}
