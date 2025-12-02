using Microsoft.AspNetCore.Authorization;

namespace TestHost.Authorization
{
    public class AlwaysAllowHandler : AuthorizationHandler<AlwaysAllowRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AlwaysAllowRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}