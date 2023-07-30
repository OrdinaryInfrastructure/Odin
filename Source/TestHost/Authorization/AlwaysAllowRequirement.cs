using Microsoft.AspNetCore.Authorization;

namespace TestHost.Authorization
{
    public class AlwaysAllowRequirement : IAuthorizationRequirement
    {
    }
}