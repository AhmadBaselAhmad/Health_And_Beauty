namespace GraduationProject.DataBase.ViewModels.DynamicAttribute
{
    public class Dynamic_AttributeViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string? Description { get; set; }
        public bool Required { get; set; }
        public bool Disable { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsHealthStandard { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType_Name { get; set; }
        public int ClinicId { get; set; }
        public string Clinic_Name { get; set; }
    }
}
