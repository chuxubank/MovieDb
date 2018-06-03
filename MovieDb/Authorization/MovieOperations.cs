using System;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace MovieDb.Authorization
{
    public static class MovieOperations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Update =
          new OperationAuthorizationRequirement { Name = Constants.UpdateOperationName };
        public static OperationAuthorizationRequirement Delete =
          new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string UpdateOperationName = "Update";
        public static readonly string DeleteOperationName = "Delete";

        public static readonly string MovieAdministratorsRole = "MovieAdministrators";
        public static readonly string MovieManagersRole = "MovieManagers";
    }
}
