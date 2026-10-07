using System.Security.Claims;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace KLMN.Infrastructure.Identity;

/// <summary>
/// Mevcut HTTP request'in JWT claim bilgilerini okur.
/// Bu servis veritabanına sorgu göndermez.
/// </summary>
internal sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    private ClaimsPrincipal? Principal =>
        httpContextAccessor.HttpContext?.User;

    public Guid? UserId =>
        Guid.TryParse(
            Principal?.FindFirstValue(
                ClaimTypes.NameIdentifier),
            out var value)
            ? value
            : null;

    public string? UserName =>
        Principal?.FindFirstValue(
            ClaimTypes.Name);

    public string? Email =>
        Principal?.FindFirstValue(
            ClaimTypes.Email);

    public string? FirstName =>
        Principal?.FindFirstValue(
            ClaimTypes.GivenName);

    public string? LastName =>
        Principal?.FindFirstValue(
            ClaimTypes.Surname);

    public string? FullName =>
        string.Join(
            " ",
            new[]
            {
                FirstName,
                LastName
            }
            .Where(x =>
                !string.IsNullOrWhiteSpace(x)));

    public Guid? OrganizationUnitId =>
        Guid.TryParse(
            Principal?.FindFirstValue(
                CustomClaimTypes.OrganizationUnitId),
            out var value)
            ? value
            : null;

    public IReadOnlyCollection<string> Roles =>
        Principal?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? [];

    public IReadOnlyCollection<string> Permissions =>
        Principal?
            .FindAll(
                CustomClaimTypes.Permission)
            .Select(x => x.Value)
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? [];

    public string? SecurityStamp =>
        Principal?.FindFirstValue(
            CustomClaimTypes.SecurityStamp);

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ==
        true;

    public string? IpAddress =>
        httpContextAccessor
            .HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();

    public string? UserAgent =>
        httpContextAccessor
            .HttpContext?
            .Request
            .Headers
            .UserAgent
            .ToString();
}
