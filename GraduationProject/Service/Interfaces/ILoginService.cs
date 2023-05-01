using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Authenticate;

namespace GraduationProject.Service.Interfaces
{
    public interface ILoginService
    {
        ApiResponse CreateToken(LoginViewModel Login);
    }
}
