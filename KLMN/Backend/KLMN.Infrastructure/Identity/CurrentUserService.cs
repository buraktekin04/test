using System.Security.Claims;
using KLMN.Application.Common.Constants;
using KLMN.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;

namespace KLMN.Infrastructure.Identity;

/// <summary>ClaimsPrincipal üzerinden mevcut kullanıcı ve istemci bilgilerini sağlar.</summary>
public sealed class CurrentUserService : ICurrentUserService
{
    /// <summary>
    /// İşlemin yürütüldüğü güncel HTTP isteğine güvenli biçimde erişim sağlar.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// current user service işlemini ilgili güvenlik ve doğrulama kurallarına uygun yürütür.
    /// </summary>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// user özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            // value bilgisini sonraki işlem adımları için hesaplar.
            var value = GetClaimValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// SMTP sunucusuna kimlik doğrulamada kullanılan hesap adıdır.
    /// </summary>
    public string? UserName => GetClaimValue(ClaimTypes.Name);
    /// <summary>
    /// email özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? Email => GetClaimValue(ClaimTypes.Email);
    /// <summary>
    /// first name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? FirstName => GetClaimValue(ClaimTypes.GivenName);
    /// <summary>
    /// last name özelliğini sınıfın veri sözleşmesinde taşır.
    /// </summary>
    public string? LastName => GetClaimValue(ClaimTypes.Surname);

    public string? FullName
    {
        get
        {
            // value bilgisini sonraki işlem adımları için hesaplar.
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
            // value bilgisini sonraki işlem adımları için hesaplar.
            var value = GetClaimValue(CustomClaimTypes.OrganizationUnitId);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    /// <summary>
    /// Oturum kullanıcısının role claim kodlarını içerir.
    /// </summary>
    public IReadOnlyCollection<string> Roles =>
        GetClaimValues(ClaimTypes.Role);

    /// <summary>
    /// Oturum kullanıcısının etkin permission claim kodlarını içerir.
    /// </summary>
    public IReadOnlyCollection<string> Permissions =>
        GetClaimValues(CustomClaimTypes.Permission);

    /// <summary>
    /// İstek kullanıcısının JWT ile doğrulanmış olup olmadığını belirtir.
    /// </summary>
    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// İstemcinin sunucuya bağlandığı IP adresidir.
    /// </summary>
    public string? IpAddress =>
        _httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();

    /// <summary>
    /// İstemcinin gönderdiği tarayıcı/cihaz tanımlayıcısıdır.
    /// </summary>
    public string? UserAgent =>
        _httpContextAccessor.HttpContext?
            .Request
            .Headers
            .UserAgent
            .FirstOrDefault();

    /// <summary>
    /// İlgili claim tipinin ilk değerini güncel HTTP kimliğinden okur.
    /// </summary>
    private string? GetClaimValue(string claimType) =>
        User?.FindFirst(claimType)?.Value;

    /// <summary>
    /// Aynı tipteki claim değerlerini tekrarsız olarak döndürür.
    /// </summary>
    private IReadOnlyCollection<string> GetClaimValues(string claimType) =>
        User?
            .FindAll(claimType)
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray()
        ?? Array.Empty<string>();
}
