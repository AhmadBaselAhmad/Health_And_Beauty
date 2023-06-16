using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Allergies;
using GraduationProject.DataBase.ViewModels.Immunization;
using GraduationProject.DataBase.ViewModels.Medicine;
using GraduationProject.DataBase.ViewModels.Surgery;

namespace GraduationProject.Service.Interfaces
{
    public interface IMedicalInformationService
    {
        ApiResponse GetAllAlergies(IFormFile File);
        ApiResponse GetAllImmunizations(IFormFile File);
        ApiResponse GetAllMedicines(IFormFile File);
        ApiResponse GetAllSurgeries(IFormFile File);
        ApiResponse AddAllergies(List<AddAllergyViewModel> NewAllergies);
        ApiResponse AddImmunizations(List<AddImmunizationViewModel> NewImmunizations);
        ApiResponse AddMedicines(List<AddMedicineViewModel> NewMedicines);
        ApiResponse AddSurgeries(List<AddSurgeryViewModel> NewSurgeries);
    }
}
