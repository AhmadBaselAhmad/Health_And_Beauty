using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Service : TimeStampModel
    {

        public int Id { get; set; }

        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public Clinic? Clinic { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Step { get; set; }
    }
}
