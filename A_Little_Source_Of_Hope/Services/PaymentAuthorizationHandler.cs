using System.Threading.Tasks;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Services
{
    public class PaymentAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Payment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Payment paymentResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.CreateOperationName)
            {
                return Task.CompletedTask;
            }
            if (context.User.IsInRole(Constants.CustomersRole) || context.User.IsInRole(Constants.OrphanageManagersRole))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}