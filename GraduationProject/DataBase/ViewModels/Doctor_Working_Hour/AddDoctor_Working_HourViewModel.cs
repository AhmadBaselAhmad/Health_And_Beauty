namespace GraduationProject.DataBase.ViewModels.Doctor_Working_Hour
{
    public class AddDoctor_Working_HourViewModel
    {
        public int WorkingDaysId { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool Off { get; set; }
    }
}
