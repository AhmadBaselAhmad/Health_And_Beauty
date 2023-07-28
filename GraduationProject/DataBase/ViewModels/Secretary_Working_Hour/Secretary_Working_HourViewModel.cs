namespace GraduationProject.DataBase.ViewModels.Secretary_Working_Hour
{
    public class Secretary_Working_HourViewModel
    {
        public int Id { get; set; }

        public int SecretaryId { get; set; }
        public string Secretary_Name { get; set; }

        public int WorkingDaysId { get; set; }
        public string WorkingDays_Name { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Off { get; set; }
    }
}
