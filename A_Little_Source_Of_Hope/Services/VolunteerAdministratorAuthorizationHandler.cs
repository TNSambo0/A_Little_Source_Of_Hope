﻿using System.Threading.Tasks;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
namespace A_Little_Source_Of_Hope.Services
{
    public class VolunteerAdministratorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Volunteer>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Volunteer volunteerResource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName &&
                requirement.Name != Constants.ReadOperationName)
            {
                return Task.CompletedTask;
            }
            if (context.User.IsInRole(Constants.VolunteerAdministratorsRole))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
