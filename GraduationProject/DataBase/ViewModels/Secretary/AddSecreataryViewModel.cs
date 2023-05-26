using GraduationProject.DataBase.ViewModels.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Secretary
{
    public class AddSecreataryViewModel
    {
        public int ClinicId { get; set; }

        // User Information..
        public AddUserViewModel UserInfo { get; set; }
    }
}
