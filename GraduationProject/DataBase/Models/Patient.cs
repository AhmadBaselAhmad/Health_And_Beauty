using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Patient: TimeStampModel
    {
        public int Id { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
