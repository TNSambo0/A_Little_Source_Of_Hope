﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public string FirstName { get; set; }   
        public string Type { get; set; }
        [NotMapped]
        public string LastName { get; set; }
        public DateTime DateCreated { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
