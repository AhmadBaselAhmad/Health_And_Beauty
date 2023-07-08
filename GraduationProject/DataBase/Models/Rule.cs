using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Rule : TimeStampModel
    {
        public int Id { get; set; }

        public int? StaticAttributeId { get; set; }
        [ForeignKey("StaticAttributeId")]
        public Static_Attribute? StaticAttribute { get; set; }

        public int? DynamicAttributeId { get; set; }
        [ForeignKey("DynamicAttributeId")]
        public Dynamic_Attribute? DynamicAttribute { get; set; }

        public int NewDynamicAttributeId { get; set; }
        [ForeignKey("NewDynamicAttributeId")]
        public Dynamic_Attribute? NewDynamicAttribute { get; set; }

        public int OperationId { get; set; }
        [ForeignKey("OperationId")]
        public Operation? Operation { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }

        public string? OperationValueString { get; set; }
        public double? OperationValueDouble { get; set; }
        public DateTime? OperationValueDateTime { get; set; }
        public bool? OperationValueBoolean { get; set; }
    }
}
