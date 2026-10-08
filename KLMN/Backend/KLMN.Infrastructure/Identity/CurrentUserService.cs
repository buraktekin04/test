using System.Security.Claims;
using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;

namespace KLMN.Infrastructure.Identity;

/// <summary>ClaimsPrincipal üzerinden mevcut kullanıcı ve istemci bilgilerini sağlar.</summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? UserName => GetClaimValue(ClaimTypes.Name);
    public string? Email => GetClaimValue(ClaimTypes.Email);
    public string? FirstName => GetClaimValue(ClaimTypes.GivenName);
    public string? LastName => GetClaimValue(ClaimTypes.Surname);

    public string? FullName
    {
        get
        {
            var value = string.Join(
                " ",
                new[] { FirstName, LastName }
                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }

    public Guid? OrganizationUnitId
    {
        get
        {
            var value = GetClaimValue(CustomClaimTypes.OrganizationUnitId);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public IReadOnlyCollection<string> Roles =>
        GetClaimValues(ClaimTypes.Role);

    public IReadOnlyCollection<string> Permissions =>
        GetClaimValues(CustomClaimTypes.Permission);

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    public string? IpAddress =>
        _httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();

    public string? UserAgent =>
        _httpContextAccessor.HttpContext?
            .Request
            .Headers
            .UserAgent
            .FirstOrDefault();

    private string? GetClaimValue(string claimType) =>
        User?.FindFirst(claimType)?.Value;

    private IReadOnlyCollection<string> GetClaimValues(string claimType) =>
        User?
            .FindAll(claimType)
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();
}
