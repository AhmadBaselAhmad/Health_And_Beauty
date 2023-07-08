using GraduationProject.DataBase.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.ViewModels.DynamicAttribute.Dependency
{
    public class AddDependencyValidationViewModel
    {
        // Selected From The Operation Drop Down List..
        public int OperationId { get; set; }

        public string? ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
    }
}
