using GraduationProject.DataBase.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Appointment
{
    public class EditAppointmentViewModel
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string Status { get; set; }
        public bool Notified { get; set; }
    }
}
