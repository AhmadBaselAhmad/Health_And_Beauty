namespace GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency
{
    public class AddDependencyGroupsRules
    {
        public int? StaticAttributeId { get; set; }
        public int? DynamicAttributeId { get; set; }
        public int ClinicId { get; set; }

        // Select From The Operation Drop Down List..
        public int OperationId { get; set; }

        // Depending On The Selected Dynamic Attribute's DataType..
        public string? OperationValueString { get; set; }
        public double? OperationValueDouble { get; set; }
        public DateTime? OperationValueDateTime { get; set; }
        public bool? OperationValueBoolean { get; set; }
    }
}
