using GraduationProject.DataBase.ViewModels.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Patient
{
    public class PatientViewModel
    {
        public int Id { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

        public UserViewModel UserInformation { get; set; }
    }
}
