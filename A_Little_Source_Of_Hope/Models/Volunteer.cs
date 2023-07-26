using System.ComponentModel.DataAnnotations;


namespace A_Little_Source_Of_Hope.Models
{
    public class Volunteer
    {
        [Key]
        public int VoluneerId { get;set;} 
        public string Description { get;set;} 
        public string OrphanageName  { get;set;} 
        public DateTime VolunteerDate  { get;set;}
        public string AppUserId { get; set; }
        public int MyProperty { get; set; }


    }
}
