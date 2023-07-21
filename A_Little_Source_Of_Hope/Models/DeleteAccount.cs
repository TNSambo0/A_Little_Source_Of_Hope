#nullable disable
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Models
{
    public class DeleteAccount
    {
        [Key]
        public int DeleteAccountId { get; set; }
        public string DeactivatingReason { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
