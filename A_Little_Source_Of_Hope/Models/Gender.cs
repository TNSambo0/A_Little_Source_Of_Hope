using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Models
{
    public class Gender
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string GenderName { get; set; }
    }
}
