using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Secretary;

namespace GraduationProject.Service.Interfaces
{
    public interface ISecretaryService
    {
        ApiResponse GetSecretaryById(int SecretaryId);
        ApiResponse DeleteSecreatry(int SecretaryId);
        ApiResponse AddNewSecretary(AddSecreataryViewModel NewSecretary);
        ApiResponse EditClinicSecretary(int SecretaryId, int NewClinicId);
        ApiResponse GetAllSecretaries(int ClinicId, ComplexFilter Filter);
    }
}
