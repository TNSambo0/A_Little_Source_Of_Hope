using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
namespace A_Little_Source_Of_Hope.Services
{
    public class VolunteerCustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Volunteer>
    {
        UserManager<AppUser> _userManager;
        public VolunteerCustomerAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Volunteer volunteerResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.ReadOperationName)
            {
                return Task.CompletedTask;
            }
            var user = await _userManager.GetUserAsync(context.User);
            //if ((user.UserType == "Customer" && volunteerResource.AppUserId == _userManager.GetUserId(context.User)) || (user.UserType == "Orphanage Manager" && shoppingCartResource.AppUserId == _userManager.GetUserId(context.User)))
            if (user.UserType == "Customer" && volunteerResource.AppUserId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
