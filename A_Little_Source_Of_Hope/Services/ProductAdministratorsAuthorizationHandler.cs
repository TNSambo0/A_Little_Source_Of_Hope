using System.Threading.Tasks;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Services
{
    public class ProductAdministratorsAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Product>
    {
        UserManager<AppUser> _userManager;
        public ProductAdministratorsAuthorizationHandler(UserManager<AppUser> userManager)
        {

            _userManager = userManager;

        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Product productResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.ReadOperationName &&
                requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if
            (context.User.IsInRole(Constants.ProductAdministratorsRole))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
