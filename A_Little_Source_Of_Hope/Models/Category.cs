using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_Little_Source_Of_Hope.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Display(Name = "Category name")]
        [DataType(DataType.Text)]
        public string CategoryName { get; set; }
        public string? Imageurl { get; set; }
        [Display(Name = "Is active")]
        [Column(TypeName = "bit")]
        public bool IsActive { get; set; }
        [Display(Name = "Created date")]
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; } 
    }
}
