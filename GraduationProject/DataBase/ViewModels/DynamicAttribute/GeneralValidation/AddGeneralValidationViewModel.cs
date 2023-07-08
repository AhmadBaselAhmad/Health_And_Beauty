namespace GraduationProject.DataBase.ViewModels.DynamicAttribute.GeneralValidation
{
    public class AddGeneralValidationViewModel
    {
        // Selected From Operation Drop Down List..
        public int OperationId { get; set; }

        // Depending On The Dynamic Attribute DataType Name We Can Know Which Attribute To Use..
        public string? ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
    }
}
