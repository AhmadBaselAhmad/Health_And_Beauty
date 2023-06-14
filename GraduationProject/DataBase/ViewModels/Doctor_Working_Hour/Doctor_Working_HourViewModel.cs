namespace GraduationProject.DataBase.ViewModels.Doctor_Working_Hour
{
    public class Doctor_Working_HourViewModel
    {
        public int Id { get; set; }

        public int WorkingDaysId { get; set; }
        public string? WorkingDays_Name { get; set; }

        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
        public bool Off { get; set; }
    }
}
