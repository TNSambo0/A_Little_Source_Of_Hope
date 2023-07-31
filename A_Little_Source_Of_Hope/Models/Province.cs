using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Models
{
    public class Province
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string ProvinceName{ get; set; }
                
    }

}
