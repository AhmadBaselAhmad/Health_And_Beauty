using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Service
{
    public class ServiceViewModel
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }
        public string? Clinic_Name { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Step { get; set; }
    }
}
