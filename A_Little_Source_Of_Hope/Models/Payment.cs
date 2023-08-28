using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_Little_Source_Of_Hope.Models
{
    public class Payment
    {
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string CardNumber { get; set; }
        [StringLength(50)]
        public string ExpiryDate { get; set; }
        public int CVVNumber { get; set; }
        public string Address { get; set; }
        [NotMapped]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }

        public decimal Amount { get; set; }
        
    }
}
