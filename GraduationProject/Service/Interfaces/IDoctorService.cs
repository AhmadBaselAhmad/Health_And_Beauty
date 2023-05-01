using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Doctor;

namespace GraduationProject.Service.Interfaces
{
    public interface IDoctorService
    {
        ApiResponse AddNewDoctor(AddDoctorViewModel NewDoctor);
    }
}
