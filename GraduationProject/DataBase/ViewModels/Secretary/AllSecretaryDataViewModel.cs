﻿using GraduationProject.DataBase.ViewModels.Doctor_Working_Hour;
using GraduationProject.DataBase.ViewModels.Secretary_Working_Hour;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Secretary
{
    public class AllSecretaryDataViewModel
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }
        public string Clinic_Name { get; set; }

        public int UserId { get; set; }
        public string User_Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone_Number { get; set; }
        public string Telephone_Number { get; set; }
        public string Email { get; set; }

        public List<Secretary_Working_HourViewModel> Secretary_Working_Hours { get; set; }
    }
}
