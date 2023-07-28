using System.ComponentModel.DataAnnotations;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
namespace A_Little_Source_Of_Hope.Models

    {
    public class Orphanage
    {
        [Key]
        public int OrphanageId { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Orphanage Name")]
        public string OrphanageName { get; set; }
        [Required]
        [DataType(DataType.Text)]
        public string Manager { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Orphanage Email")]
        public string OrphanageEmail { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Physical Address")]
        public string OrphanageAddress { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Cell No")]
        public string  CellNumber { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
