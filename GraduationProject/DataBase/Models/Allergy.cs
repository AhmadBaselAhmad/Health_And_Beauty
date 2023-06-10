using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Allergy : TimeStampModel
    {
        public int Id { get; set; }
        public int MedicalInfoId { get; set; }
        public string Type { get; set; }
        public string AllergicTo { get; set; }
        public string Description { get; set; }

        [ForeignKey("MedicalInfoId")]
        public Medical_Information? MedicalInfo { get; set; }
    }
}
