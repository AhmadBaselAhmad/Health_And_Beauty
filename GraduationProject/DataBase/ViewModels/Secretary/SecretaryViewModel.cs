using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Secretary
{
    public class SecretaryViewModel
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }
        public string Clinic_Name { get; set; }

        public int UserId { get; set; }
        public string User_Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
