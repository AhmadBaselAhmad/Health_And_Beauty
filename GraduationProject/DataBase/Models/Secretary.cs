using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Secretary : TimeStampModel
    {
        [Key]
        public int Id { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
