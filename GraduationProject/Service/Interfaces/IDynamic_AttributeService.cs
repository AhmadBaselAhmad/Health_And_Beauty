using GraduationProject.DataBase.Helpers;

namespace GraduationProject.Service.Interfaces
{
    public interface IDynamic_AttributeService
    {
        ApiResponse GetDynamicAttributeById(int Dynamic_AttributeId);
        ApiResponse GetAllDynamicAttributes(ComplexFilter Filter, bool? OnlyHealthStandards);
        ApiResponse GetAllOperations();
        ApiResponse GetAllDataTypes();
    }
}
