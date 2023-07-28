using GraduationProject.DataBase.ViewModels.Secretary_Working_Hour;
using GraduationProject.DataBase.ViewModels.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Secretary
{
    public class AddSecreataryViewModel
    {
        public int ClinicId { get; set; }

        // User Information..
        public AddUserViewModel UserInfo { get; set; }

        public List<AddSecretary_Working_HourViewModel> Secretary_Working_Hours { get; set; }
    }
}
