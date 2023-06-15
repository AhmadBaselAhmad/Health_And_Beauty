using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Doctor
{
    public class EditDoctorViewModel
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public string Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }
        public List<Doctor_Working_HourViewModel> Doctor_Working_Hours { get; set; }
    }
}
