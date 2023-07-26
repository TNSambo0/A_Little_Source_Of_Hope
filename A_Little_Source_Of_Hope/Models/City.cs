using System.ComponentModel.DataAnnotations;

namespace A_Little_Source_Of_Hope.Models
{
    public class City
    {
        [Key]
        public int cityId { get; set; }
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Text)]

        public string city { get; set; }
        public int provinceId { get; set; }
        public Province Province { get; set; }
    }
}
