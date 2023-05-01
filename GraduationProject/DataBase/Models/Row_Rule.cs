using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace GraduationProject.DataBase.Models
{
    public class Row_Rule : TimeStampModel
    {

        public int Id { get; set; }

        public int RowId { get; set; }
        [ForeignKey("RowId")]
        public Row? Row { get; set; }

        public int RuleId { get; set; }
        [ForeignKey("RuleId")]
        public Rule? Rule { get; set; }
    }
}
