using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_Little_Source_Of_Hope.Models
{
    public class AdminDashboard
    {
        public int NumberofProducts { get; set; }
        public int NumberofOrders { get; set; }
        public int NumberofOrphanages { get; set; }
        public int DonatedAmount { get; set; }
        public IEnumerable<NewsSubscription> SubscribersList { get; set; }
    }
}
