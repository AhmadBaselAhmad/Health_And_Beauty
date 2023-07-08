using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Dynamic_Attribute : TimeStampModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string? Description { get; set; }
        public bool Required { get; set; }
        public bool Disable { get; set; }
        public string? DefaultValue { get; set; }
        public bool IsHealthStandard { get; set; }

        public int DataTypeId { get; set; }
        [ForeignKey("DataTypeId")]
        public Data_Type? DataType { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }
    }
}
