using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;

namespace GraduationProject.Service.Interfaces
{
    public interface IDoctorService
    {
        ApiResponse AddNewDoctor(AddDoctorViewModel NewDoctor);
        ApiResponse GetAllDoctors(ComplexFilter Filter);
        ApiResponse EditDoctor(EditDoctorViewModel DoctorNewData);
        ApiResponse GetDoctorById(int DoctorId);
        ApiResponse DeleteDoctor(int DoctorId);
    }
}
