namespace GraduationProject.DataBase.ViewModels.Surgery
{
    public class AddSurgeryViewModel
    {
        public int MedicalInfoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeOnly Date { get; set; }
    }
}
