using KLMN.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace KLMN.Infrastructure.Authorization;

/// <summary>"Permission:" prefix'li policy'leri çalışma zamanında üretir.</summary>
public sealed class PermissionAuthorizationPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    /// <summary>
    /// permission authorization policy provider işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public PermissionAuthorizationPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    /// <summary>
    /// Permission ön ekli policy adlarından dinamik authorization kuralları oluşturur.
    /// </summary>
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(
                AuthorizationPolicyNames.PermissionPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            return base.GetPolicyAsync(policyName);
        }

        // permission code bilgisini sonraki işlem adımları için hesaplar.
        var permissionCode =
            policyName[AuthorizationPolicyNames.PermissionPrefix.Length..];

        if (string.IsNullOrWhiteSpace(permissionCode))
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        // policy bilgisini sonraki işlem adımları için hesaplar.
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissionCode))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
