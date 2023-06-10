using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Service
{
    public class AddClinicServiceViewModel
    {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Step { get; set; }
    }
}
