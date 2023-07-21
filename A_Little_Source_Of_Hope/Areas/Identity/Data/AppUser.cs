using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Areas.Identity.Data
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [StringLength(50)]
        public string? FirstName { get; set; }
        [PersonalData]
        [StringLength(50)]
        public string? LastName { get; set; }
        [PersonalData]
        [StringLength(50)]
        public string? Gender { get; set; }
        [PersonalData]
        public string? AddressLine1 { get; set; }
        [PersonalData]
        public string? AddressLine2 { get; set; }
        [PersonalData]
        [StringLength(250)]
        public string? City { get; set; }
        [PersonalData]
        [StringLength(250)]
        public string? Province { get; set; }
        [PersonalData]
        [StringLength(50)]
        public string? PostalCode { get; set; }
        [PersonalData]
        public string? ImageUrl { get; set; }
        [StringLength(100)]
        public string? UserType { get; set; }
    }
}
