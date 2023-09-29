using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_Little_Source_Of_Hope.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Volunteering date")]
        public DateTime? VolunteerDate { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? OrphanageList { get; set; }
        [Display(Name = "Orphanages")]
        [NotMapped]
        public string OrphanageID { get; set; }
        [ForeignKey("Orphanage")]
        public int OrphanageId { get; set; }
        public Orphanage? Orphanage { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
