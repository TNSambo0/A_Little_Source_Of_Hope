using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_Little_Source_Of_Hope.Models
{
    public class DeleteAccount
    {
        [Key]
        public int Id { get; set; }
        public string DeactivatingReason { get; set; }
        public string Username { get; set; }
        [ForeignKey("AppUser")]
        public string UserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
