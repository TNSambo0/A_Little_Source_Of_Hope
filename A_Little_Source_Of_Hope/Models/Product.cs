using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_Little_Source_Of_Hope.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        [Display(Name = "Product name")]
        [DataType(DataType.Text)]
        public string ProductName { get; set; }
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [Range(1,100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Imageurl { get; set; }
        [ForeignKey("Category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [Column(TypeName = "bit")]
        [Display(Name = "Is active")]
        public bool IsActive { get; set; }
        [Column(TypeName = "bit")]
        [Display(Name = "Claim status")]
        public bool ClaimStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> CategoryNames { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public Category Category { get; set; }
    }
}
