using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IAppointmentService
    {
        ApiResponse GetAllAppointmentsForDoctorRole(int UserId, string AppointmentStatus, ComplexFilter Filter);
        ApiResponse GetAllAppointmentsForSecretaryRole(int DoctorId, string AppointmentStatus, ComplexFilter Filter);
        ApiResponse ChangeAppointmentStatus(int AppointmentId, string NewAppointmentStatus);
    }
}
