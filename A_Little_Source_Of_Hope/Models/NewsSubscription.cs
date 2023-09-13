using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Models
{
    public class NewsSubscription
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(250)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Column(TypeName = "bit")]
        public bool Subscribed { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
