using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IMedicalInformationService
    {
        ApiResponse GetAllAlergies(IFormFile File);
        ApiResponse GetAllImmunizations(IFormFile File);
        ApiResponse GetAllMedicines(IFormFile File);
        ApiResponse GetAllSurgeries(IFormFile File);
    }
}
