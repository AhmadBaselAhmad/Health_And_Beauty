namespace GraduationProject.DataBase.ViewModels.Secretary_Working_Hour
{
    public class AddSecretary_Working_HourViewModel
    {
        public int WorkingDaysId { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool Off { get; set; }
    }
}
