using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_Little_Source_Of_Hope.Models
{
    public class Volunteer
    {
        [Key]
        public int Id { get;set;} 
        public string? Status  { get;set;}
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? VolunteerDate { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage name")]
        public string? OrphanageName { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage address")]
        public string? OrphanageAddress { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage Email")]
        public string? OrphanageEmail { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage manager")]
        public string? OrphanageManager { get; set; }
        [NotMapped]
        [Display(Name = "Orphanage Contact")]
        public string? OrphanageContact { get; set; }
        [NotMapped]
        [Display(Name = "Applicant full name")]
        public string? ApplicantFullName { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> OrphanageList { get; set; }
        [NotMapped]
        public string OrphanageID { get; set; }
        [ForeignKey("Orphanage")]
        public int OrphanageId { get; set; }
        public virtual Orphanage Orphanage { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
