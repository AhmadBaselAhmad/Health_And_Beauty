using GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency;
using GraduationProject.DataBase.ViewModels.DynamicAttribute.GeneralValidation;

namespace GraduationProject.DataBase.ViewModels.DynamicAttribute
{
    public class AddDynamic_AttributeViewModel
    {
        //
        // General Information..
        //
        public string Key { get; set; }
        public string? Description { get; set; }
        public bool Required { get; set; }

        public string? DefaultValue { get; set; }
        public bool IsHealthStandard { get; set; }

        // Select From Data Type Drop Down List..
        public int DataTypeId { get; set; }

        // Send It From The Header Token (From Front [ClinicId], From Back [Full Token])
        public int ClinicId { get; set; }

        //
        // General Validation..
        //

        public AddGeneralValidationViewModel GeneralValidation { get; set; }

        //
        // Dependency Full Information..
        //
        public AddFullDependencyInfoViewModel Dependency { get; set; }
    }
}
