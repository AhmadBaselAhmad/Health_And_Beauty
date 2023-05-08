namespace GraduationProject.DataBase.ViewModels.DynamicAttribute
{
    public class AddRuleViewModel
    {
        public int? AttributeActivatedId { get; set; }
        public int? DynamicAttId { get; set; }
        public int OperationId { get; set; }
        public string OperationValueString { get; set; }
        public bool IsDynamic { get; set; }
    }
}
