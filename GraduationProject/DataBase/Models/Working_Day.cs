using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Working_Day : TimeStampModel
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public bool Off { get; set; }
    }
}
