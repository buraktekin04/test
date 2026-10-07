using KLMN.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Authorization;

/// <summary>
/// Permission:&lt;code&gt; formatındaki policy'leri çalışma zamanında üretir.
/// </summary>
internal sealed class PermissionAuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    /// <inheritdoc />
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (!policyName.StartsWith(
                AuthorizationPolicyNames.PermissionPrefix,
                StringComparison.Ordinal))
        {
            return await base.GetPolicyAsync(
                policyName);
        }

        var permissionCode =
            policyName[
                AuthorizationPolicyNames
                    .PermissionPrefix
                    .Length..];

        if (string.IsNullOrWhiteSpace(
            permissionCode))
        {
            return null;
        }

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(
                new PermissionRequirement(
                    permissionCode))
            .Build();
    }
}
