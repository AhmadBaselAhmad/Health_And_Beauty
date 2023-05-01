using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Surgery : TimeStampModel
    {

        public int Id { get; set; }

        public int MedicalInfoId { get; set; }
        [ForeignKey("MedicalInfoId")]
        public Medical_Information? MedicalInfo { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public TimeOnly Date { get; set; }
    }
}
