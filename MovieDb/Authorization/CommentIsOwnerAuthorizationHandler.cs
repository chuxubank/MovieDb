using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using MovieDb.Data;

namespace MovieDb.Authorization
{
    public class CommentIsOwnerAuthorizationHandler 
        : AuthorizationHandler<OperationAuthorizationRequirement, Comment>
    {
        UserManager<ApplicationUser> _userManager;
        public CommentIsOwnerAuthorizationHandler (UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context,
                                   OperationAuthorizationRequirement requirement,
                                   Comment resource)
        {
            if (context.User == null || resource == null)
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

            if (resource.UserID == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
