using System.ComponentModel.DataAnnotations;
namespace A_Little_Source_Of_Hope.Models
{
    public class AdminDashboard
    {
        [Display(Name = "Products")]
        public int NumberofProducts { get; set; }
        [Display(Name = "Orders")]
        public int NumberofOrders { get; set; }
        [Display(Name = "Orphanages")]
        public int NumberofOrphanages { get; set; }
        [Display(Name = "Current Cash Raised")]
        public decimal DonatedAmount { get; set; }
        public IEnumerable<NewsSubscription> SubscribersList { get; set; }
        public VolunteerAdminDashboard VolunteerApps { get; set; }
    }
}
