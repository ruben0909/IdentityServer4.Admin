using Microsoft.AspNetCore.Authorization;
using Skoruba.IdentityServer4.Admin.Configuration.Constants;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Policies
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AdminAuthorizationRequirement requirement)
        {


            if (context.User.IsInRole(AuthorizationConsts.AdministrationRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
