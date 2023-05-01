﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Validation : TimeStampModel
    {

        public int Id { get; set; }

        public int DynamicAttributeId { get; set; }
        [ForeignKey("DynamicAttributeId")]
        public Medical_Information? DynamicAttribute { get; set; }
        
        public int OperationId { get; set; }
        [ForeignKey("OperationId")]
        public Operation? Operation { get; set; }

        public string? ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
    }
}
