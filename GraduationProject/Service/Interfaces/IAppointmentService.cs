using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Prescription;

namespace GraduationProject.Service.Interfaces
{
    public interface IAppointmentService
    {
        ApiResponse GetAllAppointmentsForDoctorRole(int UserId, string AppointmentStatus, ComplexFilter Filter);
        ApiResponse GetAllAppointmentsForSecretaryRole(int DoctorId, string AppointmentStatus, ComplexFilter Filter);
        ApiResponse ChangeAppointmentStatus(int AppointmentId, string NewAppointmentStatus);
        ApiResponse AddPrescription(AddPrescriptionViewModel NewPrescription);
        ApiResponse EditPrescription(EditPrescriptionViewModel NewPrescriptionData);
    }
}
