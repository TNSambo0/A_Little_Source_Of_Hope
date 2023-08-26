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
        [Display(Name = "Orphanage Name")]
        public string OrphanageName { get; set; }
        [NotMapped]
        public string? Manager { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Managers { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage Email")]
        public string? OrphanageEmail { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Physical Address")]
        public string OrphanageAddress { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Cell No")]
        public string  CellNumber { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
