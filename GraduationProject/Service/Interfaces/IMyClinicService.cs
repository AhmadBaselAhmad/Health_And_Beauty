using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Service;

namespace GraduationProject.Service.Interfaces
{
    public interface IMyClinicService
    {
        ApiResponse GetAllClinicServices(ComplexFilter Filter, int? ClinicId);
        ApiResponse AddNewClinicService(AddClinicServiceViewModel AddClinicServiceViewModel);
        ApiResponse GetClinicServiceById(int ClinicServiceId);
        ApiResponse EditClinicService(EditClinicServiceViewModel EditClinicServiceViewModel);
        ApiResponse DeleteClinicService(int ClinicServiceId);
    }
}
