﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Data_Type: TimeStampModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
