using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IMyClinicService
    {
        ApiResponse GetAllClinicServices(int ClinicId, ComplexFilter Filter);
    }
}
