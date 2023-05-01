using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Operation: TimeStampModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
