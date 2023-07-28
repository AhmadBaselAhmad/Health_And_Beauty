using GraduationProject.DataBase.ViewModels.Secretary_Working_Hour;

namespace GraduationProject.DataBase.ViewModels.Secretary
{
    public class EditSecretaryViewModel
    {
        public int Id { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }

        public List<Secretary_Working_HourViewModel> Secretary_Working_Hours { get; set; }
    }
}
