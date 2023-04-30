using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationProject.DataBase.Models
{
    public class Dependency_Row: TimeStampModel
    {
        public int Id { get; set; }

        public int DependencyId { get; set; }
        [ForeignKey("DependencyId")]
        public Dependency? Dependency { get; set; }

        public int RowId { get; set; }
        [ForeignKey("RowId")]
        public Row? Row { get; set; }
    }
}
