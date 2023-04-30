using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Notification : TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }

        public int? FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public User? FromUser { get; set; }

        public int ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public User? ToUser { get; set; }

    }
}
