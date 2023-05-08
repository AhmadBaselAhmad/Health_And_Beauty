namespace GraduationProject.DataBase.ViewModels.DynamicAttribute
{
    public class DependencyViewModel
    {
        public List<AddRuleViewModel> DependencyRules { get; set; }
        public int OperationId { get; set; }
        public string? ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
    }
}
