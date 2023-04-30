using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class User: TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? Blocked_Date { get; set; }
    }
}
