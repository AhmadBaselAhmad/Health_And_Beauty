using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Doctor: TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string Degree { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool IsHeadOfClinic { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }
    }
}
