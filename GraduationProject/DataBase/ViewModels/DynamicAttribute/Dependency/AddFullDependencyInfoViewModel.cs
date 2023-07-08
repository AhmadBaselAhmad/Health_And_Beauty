namespace GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency
{
    public class AddFullDependencyInfoViewModel
    {
        // Dependency Value..
        public string? DependencyValue { get; set; }

        // Dependency Validation..
        public AddDependencyValidationViewModel? DependencyValidation { get; set; }

        // Dependency Rules..
        public List<List<AddDependencyGroupsRules>>? DependencyGroupsRules { get; set; }
    }
}
