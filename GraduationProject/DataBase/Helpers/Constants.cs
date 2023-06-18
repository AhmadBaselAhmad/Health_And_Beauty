using System.ComponentModel;

namespace GraduationProject.DataBase.Helpers
{
    public class Constants
    {
        public enum Roles
        {
            Doctor,
            Secretary,
            SuperAdmin,
            ClinicAdmin
        }
        public enum AppointmentStatus
        {
            pending = 0,
            complete = 1,
            cancel = 2
        }
        public enum BGroups
        {
            [Description("A+ B+ O+ AB+ A- B- O- AB-")]
            BGroups
        }
    }
}
