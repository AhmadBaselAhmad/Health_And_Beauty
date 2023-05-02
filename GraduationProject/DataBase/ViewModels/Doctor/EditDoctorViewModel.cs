using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Doctor
{
    public class EditDoctorViewModel
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool IsHeadOfClinic { get; set; }

        public int UserId { get; set; }
        public int ClinicId { get; set; }
    }
}
