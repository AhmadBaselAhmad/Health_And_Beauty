using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.ViewModels.Authenticate
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
