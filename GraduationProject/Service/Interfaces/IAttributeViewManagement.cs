using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IAttributeViewManagement
    {
        ApiResponse GetAllAttributeViewManagement(int ClinicId, ComplexFilter Filter);
        ApiResponse UpdateAttributeStatus(int AttributeViewManagementId);
    }
}
