using System.ComponentModel.DataAnnotations;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;


namespace A_Little_Source_Of_Hope.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        [NotMapped]
        public string ImageUrl { get; set; }
        [NotMapped]
        public int AvailableQuantity { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string Description { get; set; }
        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal PricePerItem { get; set; }
        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal TotalPerItem { get; set; }
        [NotMapped]
        [DataType(DataType.Currency)]
        public decimal Total { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; }

        public Product? Product { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
