using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Secretary_Working_Hour : TimeStampModel
    {
        public int Id { get; set; }

        public int SecretaryId { get; set; }
        [ForeignKey("SecretaryId")]
        public User? Secretary { get; set; }

        public int WorkingDaysId { get; set; }
        [ForeignKey("WorkingDaysId")]
        public Working_Day? WorkingDays { get; set; }

        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool Off { get; set; }
    }
}
