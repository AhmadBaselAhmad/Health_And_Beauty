using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Immunization
{
    public class AddImmunizationViewModel
    {
        public int MedicalInfoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; }
    }
}
