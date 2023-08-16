using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Prescription : TimeStampModel
    {
        public int Id { get; set; }

        public int AppointmentId { get; set; }
        [ForeignKey("AppointmentId")]
        public Appointment? Appointment { get; set; }

        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }

        public int MedicalInfoId { get; set; }
        [ForeignKey("MedicalInfoId")]
        public Medical_Information? MedicalInfo { get; set; }
    }
}
