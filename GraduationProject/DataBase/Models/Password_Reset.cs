using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace GraduationProject.DataBase.Models
{
    public class Password_Reset
    {
        [Key]
        public string Email { get; set; }
        public string Token { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        public DateTime? Created_At { get; set; }
    }
}
