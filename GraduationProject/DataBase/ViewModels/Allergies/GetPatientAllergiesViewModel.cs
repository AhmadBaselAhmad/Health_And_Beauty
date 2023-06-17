using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Allergies
{
    public class GetPatientAllergiesViewModel
    {
        public int Id { get; set; }
        public int MedicalInfoId { get; set; }
        public string Type { get; set; }
        public string AllergicTo { get; set; }
        public string Description { get; set; }
    }
}
