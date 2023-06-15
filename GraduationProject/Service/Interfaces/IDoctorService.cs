using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;

namespace GraduationProject.Service.Interfaces
{
    public interface IDoctorService
    {
        ApiResponse AddNewDoctor(AddDoctorViewModel NewDoctor);
        ApiResponse GetAllDoctors(ComplexFilter Filter, int? ClinicId);
        ApiResponse EditDoctor(EditDoctorViewModel DoctorNewData);
        ApiResponse GetDoctorById(int UserId);
        ApiResponse DeleteDoctor(int DoctorId);
    }
}
