using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Medical_Information : TimeStampModel
    {

        public int Id { get; set; }

        public int PatientId { get; set; }
        [ForeignKey("PatientId")]
        public User? Patient { get; set; }

        public string Height { get; set; }
        public string BGroup { get; set; }
        public string Pulse { get; set; }
        public string? Allergy { get; set; }
        public string Weight { get; set; }
        public string BPressure { get; set; }
        public string Respiration { get; set; }
        public string Diet { get; set; }
    }
}
