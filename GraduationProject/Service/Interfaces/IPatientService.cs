using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.Medical_Information;

namespace GraduationProject.Service.Interfaces
{
    public interface IPatientService
    {
        ApiResponse AddPatientMedicalInfo(AddMedical_InformationsViewModel PatientMedicalInformationViewModel);
        ApiResponse EditPatientMedicalInfo(EditMedical_InformationsViewModel PatientMedicalInformationViewModel);
        ApiResponse GetMedicalInformationByPatientId(int PatientId);
        ApiResponse GetPatientById(int PatientId);
        ApiResponse GetAllDoctorsPatients(int UserId, ComplexFilter Filter);
    }
}
