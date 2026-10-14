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
        var permissionCodes =
            policyName[AuthorizationPolicyNames.PermissionPrefix.Length..]
                .Split('|', StringSplitOptions.TrimEntries);

        // Geçersiz veya boş permission kodları için policy oluşturulmaz.
        if (permissionCodes.Length == 0 || permissionCodes.Any(string.IsNullOrWhiteSpace))
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        // Bir kod için mevcut davranış; birden fazla kod için OR yetkilendirmesi.
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionRequirement(permissionCodes))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
