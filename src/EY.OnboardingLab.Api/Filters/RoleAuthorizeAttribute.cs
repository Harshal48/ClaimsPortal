using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EY.OnboardingLab.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly HashSet<string> _roles;

    public RoleAuthorizeAttribute(params string[] roles)
    {
        _roles = roles
            .Select(r => r.Trim())
            .Where(r => r.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();   //401 Unauthorized
            return;
        }

        if (_roles.Count == 0)
            return;

        var ok = _roles.Any(role => user.IsInRole(role));
        if (!ok)
            context.Result = new ForbidResult();   //403 Forbidden
    }
}

