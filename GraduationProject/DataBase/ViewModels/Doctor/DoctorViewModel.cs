using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;

namespace GraduationProject.DataBase.ViewModels.Doctor
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool IsHeadOfClinic { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }

        public int ClinicId { get; set; }
        public string Clinic_Name { get; set; }

        public List<Doctor_Working_HourViewModel> Doctor_Working_Hours { get; set; }
    }
}
