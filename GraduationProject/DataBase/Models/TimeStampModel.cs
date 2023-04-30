using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class TimeStampModel
    {
        public bool IsDeleted { get; set; }
        [Timestamp]
        public DateTime Created_At { get; set; }
        [Timestamp]
        public DateTime Updated_At { get; set; }
    }
}
