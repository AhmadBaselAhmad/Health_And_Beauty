using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Appointment: TimeStampModel
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Status { get; set; }
        public bool Notified { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public User? Patient { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public User? Doctor { get; set; }
    }
}
