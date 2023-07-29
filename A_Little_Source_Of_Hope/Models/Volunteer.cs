#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Models
{
    public class Volunteer
    {
        [Key]
        public int VoluneerId { get;set;} 
        public string Description { get;set;} 
        public string OrphanageName  { get;set;} 
        public string Status  { get;set;} 
        public DateTime VolunteerDate  { get;set;}
        [NotMapped]
        public string Date { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
