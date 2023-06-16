using GraduationProject.DataBase.Models;
using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;
using GraduationProject.DataBase.ViewModels.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Doctor
{
    public class AddDoctorViewModel
    {
        public string Degree { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool IsHeadOfClinic { get; set; }

        // User Information..
        public AddUserViewModel UserInfo { get; set; }

        public int ClinicId { get; set; }
    }
}
