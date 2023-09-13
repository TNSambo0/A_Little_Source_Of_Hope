using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_Little_Source_Of_Hope.Models
{
    public class VolunteerApp
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        [Display(Name = "Volunteering date")]
        public DateTime VolunteerDate { get; set; }
        [Display(Name = "Orphanage name")]
        public string OrphanageName { get; set; }
        [Display(Name = "Orphanage address")]
        public string OrphanageAddress { get; set; }
        [Display(Name = "Orphanage Email")]
        public string OrphanageEmail { get; set; }
        [Display(Name = "Orphanage manager")]
        public string OrphanageManager { get; set; }
        [Display(Name = "Orphanage Contact")]
        public string OrphanageContact { get; set; }
        [Display(Name = "Applicant full name")]
        public string ApplicantFullName { get; set; }
        public IEnumerable<SelectListItem> OrphanageList { get; set; }
        public string OrphanageID { get; set; }
        public string AppUserId { get; set; }
    }
}
