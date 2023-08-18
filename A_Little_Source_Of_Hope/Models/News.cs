using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
namespace A_Little_Source_Of_Hope.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        public string Heading { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
