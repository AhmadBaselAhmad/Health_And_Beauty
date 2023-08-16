using GraduationProject.DataBase.ViewModels.Medicine;

namespace GraduationProject.DataBase.ViewModels.Prescription
{
    public class AddPrescriptionViewModel
    {
        public int AppointmentId { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public int MedicalInfoId { get; set; }
        public List<AddMedicineViewModel> Medicines { get; set; }
    }
}
