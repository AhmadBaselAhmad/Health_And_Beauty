using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IWorking_DaysService
    {
        ApiResponse GetAllWorkingDays(ComplexFilter Filter);
        ApiResponse UpdateWorking_DayStatus(int Working_DayId);
    }
}
