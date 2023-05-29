using GraduationProject.DataBase.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.Attribute_View_Management
{
    public class AttributeViewManagementViewModel
    {
        public int Id { get; set; }
        public bool Enable { get; set; }

        public int? StaticAttributeId { get; set; }
        public string? StaticAttribute_Name { get; set; }

        public int? DynamicAttributeId { get; set; }
        public string? DynamicAttribute_Name { get; set; }
    }
}
