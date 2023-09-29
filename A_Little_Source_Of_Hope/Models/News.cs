using System.ComponentModel.DataAnnotations;
namespace A_Little_Source_Of_Hope.Models

{
    public class News
    {
        [Key]
        public int Id { get; set; }
        public string Heading { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        [Display(Name = "Created date")]
        public DateTime CreatedDate { get; set; }
    }
}
