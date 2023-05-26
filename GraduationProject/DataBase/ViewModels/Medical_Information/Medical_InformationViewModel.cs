using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Medical_Information
{
    public class Medical_InformationViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string Patient_Name { get; set; }
        public string Height { get; set; }
        public string BGroup { get; set; }
        public string Pulse { get; set; }
        public string Weight { get; set; }
        public string BPressure { get; set; }
        public string Respiration { get; set; }
        public string Diet { get; set; }
    }
}
