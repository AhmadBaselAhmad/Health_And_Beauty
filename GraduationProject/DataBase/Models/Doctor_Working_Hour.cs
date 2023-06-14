using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Doctor_Working_Hour : TimeStampModel
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public User? Doctor { get; set; }

        public int WorkingDaysId { get; set; }
        [ForeignKey("WorkingDaysId")]
        public Working_Day? WorkingDays { get; set; }

        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool Off { get; set; }
    }
}
