using GraduationProject.DataBase.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Medicine
{
    public class GetPatientMedicinesViewModel
    {
        public int Id { get; set; }
        public int? PrescriptionId { get; set; }
        public int MedicalInfoId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
