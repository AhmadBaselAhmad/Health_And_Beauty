namespace GraduationProject.DataBase.ViewModels.DynamicAttribute
{
    public class AddDependencyViewModel
    {
        public string Key { get; set; }
        public string? Description { get; set; }
        public int ClinicId { get; set; }
        public bool Required { get; set; }
        public bool Disable { get; set; }
        public bool IsHealthStandard { get; set; }
        public int DataTypeId { get; set; }

        // General Default Value...
        public string? StringDefaultValue { get; set; }
        public double? DoubleDefaultValue { get; set; }
        public DateTime? DateTimeDefaultValue { get; set; }
        public bool? BooleanDefaultValue { get; set; }

        // After Selecting Dependency and User Want to Select New Result (Another DefaultValue)...
        public string? StringResult { get; set; }

        public List<ValidationViewModel>? Validations { get; set; }
        public List<DependencyViewModel>? Dependencies { get; set; }
    }
}
