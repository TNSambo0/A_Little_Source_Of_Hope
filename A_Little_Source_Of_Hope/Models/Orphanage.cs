using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace A_Little_Source_Of_Hope.Models

    {
    public class Orphanage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Orphanage name")]
        public string OrphanageName { get; set; }
        [NotMapped]
        public string? Manager { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Managers { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage email")]
        public string? OrphanageEmail { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Orphanage physical address")]
        public string OrphanageAddress { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile No")]
        public string  PhoneNumber { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
