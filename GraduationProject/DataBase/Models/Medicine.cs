using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Medicine : TimeStampModel
    {
        public int Id { get; set; }

        public int? PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public Prescription? Prescription { get; set; }

        public int MedicalInfoId { get; set; }
        [ForeignKey("MedicalInfoId")]
        public Medical_Information? MedicalInfo { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
