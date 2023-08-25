using System.Threading.Tasks;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Humanizer.Localisation;

namespace A_Little_Source_Of_Hope.Services
{
    public class ShoppingCartCustomerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ShoppingCart>
    {
        UserManager<AppUser> _userManager;
        public ShoppingCartCustomerAuthorizationHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, ShoppingCart shoppingCartResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.AddOperationName &&
                requirement.Name != Constants.DeleteOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName)
            {
                return Task.CompletedTask;
            }
            if ((shoppingCartResource.AppUserId == _userManager.GetUserId(context.User) && context.User.IsInRole(Constants.CustomersRole)) || 
                (shoppingCartResource.AppUserId == _userManager.GetUserId(context.User) && context.User.IsInRole(Constants.OrphanageManagersRole)))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
