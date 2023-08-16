using GraduationProject.DataBase.ViewModels.Medicine;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Prescription
{
    public class EditPrescriptionViewModel
    {
        public int Id { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
    }
}
