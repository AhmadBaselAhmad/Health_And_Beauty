using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace GraduationProject.DataBase.Models
{
    public class Dynamic_Attribute_Value : TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string? ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }

        public int DynamicAttributeId { get; set; }
        [ForeignKey("DynamicAttributeId")]
        public Dynamic_Attribute? DynamicAttribute { get; set; }

        public bool Disable { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }
    }
}
