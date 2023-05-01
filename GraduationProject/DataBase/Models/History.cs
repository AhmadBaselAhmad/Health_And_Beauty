using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace GraduationProject.DataBase.Models
{
    public class History : TimeStampModel
    {

        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public string ApiUrl { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
    }
}
