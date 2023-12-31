﻿using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace A_Little_Source_Of_Hope.Data
{
    public class Operations
    {
        public static OperationAuthorizationRequirement Add = new() { Name = Constants.AddOperationName };
        public static OperationAuthorizationRequirement Create = new() { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Read = new() { Name = Constants.ReadOperationName };
        public static OperationAuthorizationRequirement Update = new() { Name = Constants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete = new() { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Approve = new() { Name = Constants.ApproveOperationName };
        public static OperationAuthorizationRequirement Reject = new() { Name = Constants.RejectOperationName };
    } 
    public class Constants
    {
        public static readonly string AddOperationName = "Add";
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationName = "Read";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";
        public static readonly string CustomersRole = "Customer";
        public static readonly string OrphanageManagersRole = "Orphanage Manager";
        public static readonly string AdministratorsRole = "Administrator";
        public static readonly string NewsLetterAdministratorsRole = "Newsletter Administrator";
        public static readonly string ProductAdministratorsRole = "Product Administrator";
        public static readonly string CategoryAdministratorsRole = "Category Administrator";
        public static readonly string OrphanageAdministratorsRole = "Orphanage Administrator";
        public static readonly string VolunteerAdministratorsRole = "Volunteer Administrator";
    }
}
