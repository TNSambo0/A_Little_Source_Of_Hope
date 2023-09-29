using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace A_Little_Source_Of_Hope.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        [Display(Name = "Card number")]
        public string CardNumber { get; set; }
        [StringLength(50)]
        [Display(Name = "Expiry date")]
        public string ExpiryDate { get; set; }
        [Display(Name = "CVV number")]
        public int CVVNumber { get; set; }
        //public string Address { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
