using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Attribute_View_Management: TimeStampModel
    {

        public int Id { get; set; }
        public bool Enable { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }

        public int? StaticAttributeId { get; set; }
        [ForeignKey("StaticAttributeId")]
        public Static_Attribute? StaticAttribute { get; set; }

        public int? DynamicAttributeId { get; set; }
        [ForeignKey("DynamicAttributeId")]
        public Dynamic_Attribute? DynamicAttribute { get; set; }
    }
}
