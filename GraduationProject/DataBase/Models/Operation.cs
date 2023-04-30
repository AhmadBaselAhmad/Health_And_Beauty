using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Operation: TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
