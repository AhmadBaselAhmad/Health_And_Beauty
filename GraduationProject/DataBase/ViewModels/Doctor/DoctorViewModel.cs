﻿namespace GraduationProject.DataBase.ViewModels.Doctor
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Doctor_Name { get; set; }
        public int ClinicId { get; set; }
        public string Clinic_Name { get; set; }

        public string Specialization { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
    }
}