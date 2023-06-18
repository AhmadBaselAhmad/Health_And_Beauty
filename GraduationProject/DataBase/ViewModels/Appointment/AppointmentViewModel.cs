using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Appointment
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string? Patient_Name { get; set; }
        public string? Patient_FirstName { get; set; }
        public string? Patient_LastName { get; set; }

        public int DoctorId { get; set; }

        public int ServiceId { get; set; }
        public string? Service_Name { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Status { get; set; }
        public bool Notified { get; set; }
    }
}
