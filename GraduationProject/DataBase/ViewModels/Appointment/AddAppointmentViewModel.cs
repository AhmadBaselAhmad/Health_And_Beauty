namespace GraduationProject.DataBase.ViewModels.Appointment
{
    public class AddAppointmentViewModel
    {
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Status { get; set; }
        public bool Notified { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
    }
}
