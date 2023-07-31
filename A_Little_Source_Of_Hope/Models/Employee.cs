using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_Little_Source_Of_Hope.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "First name")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        [StringLength(50)]
        public string? LastName { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        [StringLength(50)]
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        [StringLength(250)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? Gender { get; set; }
        [Required]
        public string? AddressLine1 { get; set; }
        [Required]
        public string? AddressLine2 { get; set; }
        [Required]
        [StringLength(250)]
        public string? City { get; set; }
        [StringLength(250)]
        public string? Province { get; set; }
        [Required]
        [StringLength(50)]
        public string? PostalCode { get; set; }
        public string? ImageUrl { get; set; }
    }
}
