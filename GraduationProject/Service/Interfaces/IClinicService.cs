using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IClinicService
    {
        ApiResponse GetAllClinics(ComplexFilter Filter);
    }
}
