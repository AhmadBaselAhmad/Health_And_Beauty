using GraduationProject.DataBase.Helpers;
using GraduationProject.DataBase.ViewModels.DynamicAttribute;

namespace GraduationProject.Service.Interfaces
{
    public interface IDynamic_AttributeService
    {
        ApiResponse GetDynamicAttributeById(int Dynamic_AttributeId);
        ApiResponse GetAllDynamicAttributes(ComplexFilter Filter, int ClinicId, bool? OnlyHealthStandards);
        ApiResponse GetAllOperations();
        ApiResponse GetAllDataTypes();
        ApiResponse AddNewDynamicAttribute(AddDynamic_AttributeViewModel NewDynamicAttribute);
        ApiResponse EditDynamicAttribute(EditDynamic_AttributeViewModel EditDynamicAttribute);
        ApiResponse ChangeDynamicAttributeRequiredStatus(int DynmaicAttributeId);
        ApiResponse ChangeDynamicAttributeDisableStatus(int DynmaicAttributeId);
        ApiResponse ChangeDynamicAttributeHealthStandardStatus(int DynmaicAttributeId);
    }
}
