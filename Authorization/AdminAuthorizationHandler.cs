using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace serverSide.Authorization
{
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAuthorizationRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                context.Fail(); // Deny access if the role claim is not "admin"
            }
            else
            {
                context.Succeed(requirement); // Allow access if the role claim is "admin"
            }

            return Task.CompletedTask;
        }
    }
}
