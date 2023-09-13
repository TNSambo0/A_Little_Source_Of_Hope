using System.ComponentModel.DataAnnotations;
namespace A_Little_Source_Of_Hope.Models
{
    public class VolunteerAdminDashboard
    {
        [Display(Name = "Approved")]
        public int NumberofApprovedApps { get; set; }
        [Display(Name = "Rejected")]
        public int NumberofRejectedApps { get; set; }
        [Display(Name = "Pending")]
        public int NumberofPendingApps { get; set; }
        [Display(Name = "Total applications")]
        public int NumbnerOfApplications { get; set; }
    }
}
