using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Models
{
    public class Province
    {
        [Key]
        public int provinceId { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Text)]

        public string province{ get; set; }
                
    }

}
