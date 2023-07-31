using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
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
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
