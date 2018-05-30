using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MovieDb.Data;

namespace MovieDb.Authorization
{
	public class MovieManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Movie>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
		                                               OperationAuthorizationRequirement requirement,
		                                               Movie resource)
		{
			if(context.User == null)
			{
				return Task.CompletedTask;
			}

			// Managers can CRUD.
			if(context.User.IsInRole(Constants.MovieManagersRole))
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}
