using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.Auditing;
using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Domain.Entities.Admissions;
using KLMN.Domain.Entities.PurchaseNumbers;
using KLMN.Domain.Entities.Geography;
using KLMN.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace KLMN.Persistence.Contexts;

/// <summary>KLMN PostgreSQL ana EF Core DbContext sınıfıdır.</summary>
public sealed class KLMNDbContext : DbContext, IKLMNDbContext
{
    /// <summary>
    /// klmndb context işlemini ilgili persistence sorumluluğuyla yerine getirir.
    /// </summary>
    public KLMNDbContext(DbContextOptions<KLMNDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Kullanıcı hesaplarına ait EF Core DbSet koleksiyonudur.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>Değiştirilemez tanımlama audit kayıtlarının sorgu kümesidir.</summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>Kullanıcı PDF metadata kayıtlarının EF Core entity kümesidir.</summary>
    public DbSet<UserAgreementPdf> UserAgreementPdfs => Set<UserAgreementPdf>();
    /// <summary>
    /// Tanımlanan dinamik ve sistem rollerinin EF Core DbSet koleksiyonudur.
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();
    /// <summary>
    /// Uygulamanın başlangıçta tanımlanan permission seed kayıtlarını içerir.
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();
    /// <summary>
    /// Kullanıcılarla roller arasındaki ilişki kayıtlarının DbSet koleksiyonudur.
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    /// <summary>
    /// Role atanmış izin ilişki kayıtlarının DbSet koleksiyonudur.
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    /// <summary>
    /// Kullanıcı bazlı izin verme veya engelleme override kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    /// <summary>
    /// Kullanıcı oturumları için hash olarak saklanan refresh token kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    /// <summary>
    /// Tek kullanımlık parola sıfırlama token kayıtlarının DbSet'idir.
    /// </summary>
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    /// <summary>
    /// Hiyerarşik birlik birimlerine ait DbSet koleksiyonudur.
    /// </summary>
    public DbSet<ForcesCommandDef> ForcesCommandDefs => Set<ForcesCommandDef>();

    /// <summary>Rapor Türü Tanımları DbSet.</summary>
    public DbSet<ReportTypeDef> ReportTypeDefs => Set<ReportTypeDef>();

    /// <summary>Rütbe Grubu Tanımları DbSet.</summary>
    public DbSet<RankGroupDef> RankGroupDefs => Set<RankGroupDef>();

    /// <summary>Rütbe Tanımları DbSet.</summary>
    public DbSet<RankDef> RankDefs => Set<RankDef>();

    /// <summary>Branş tanımlarının sorgu ve değişiklik kümesi.</summary>
    public DbSet<BranchDef> BranchDefs => Set<BranchDef>();

    /// <summary>Bağlı kurul kararlarının sorgu ve komut DbSet kümesidir.</summary>
    public DbSet<HcConnectedDecisionDef> HcConnectedDecisionDefs => Set<HcConnectedDecisionDef>();

    /// <summary>Başvuru grubu tanımlarının DbSet kümesidir.</summary>
    public DbSet<HcAdmissionTypeDef> HcAdmissionTypeDefs => Set<HcAdmissionTypeDef>();

    /// <summary>Askerlik Şubesi Tanımları veri kümesi.</summary>
    public DbSet<MilitaryOfficeDef> MilitaryOfficeDefs => Set<MilitaryOfficeDef>();

    /// <summary>Askerî Okul Tanımları veri kümesi.</summary>
    public DbSet<MilitarySchoolDef> MilitarySchoolDefs => Set<MilitarySchoolDef>();

    /// <summary>Askerî Sınıf Tanımları veri kümesi.</summary>
    public DbSet<MilitaryClassDef> MilitaryClassDefs => Set<MilitaryClassDef>();

    /// <summary>Askerî sınıf grupları EF Core veri kümesi.</summary>
    public DbSet<MilitaryClassGroupDef> MilitaryClassGroupDefs => Set<MilitaryClassGroupDef>();

    /// <summary>Sınıf-grup-kuvvet eşleştirmeleri EF Core veri kümesi.</summary>
    public DbSet<MilitaryClassGroupRelation> MilitaryClassGroupRelations => Set<MilitaryClassGroupRelation>();

    /// <summary>Alım numarası tanımlarının veri kümesi.</summary>
    public DbSet<PurchaseNumberDef> PurchaseNumberDefs => Set<PurchaseNumberDef>();
    public DbSet<CountryDef> CountryDefs => Set<CountryDef>();
    public DbSet<CityDef> CityDefs => Set<CityDef>();

    /// <summary>İlçe Tanımları DbSet; EF Core entity ilişkilerini içerir.</summary>
    public DbSet<TownDef> TownDefs => Set<TownDef>();

    public DbSet<MilitaryUnitDef> MilitaryUnitDefs => Set<MilitaryUnitDef>();

    /// <summary>
    /// Entity konfigürasyonlarını assembly'den yükleyip global sorgu filtrelerini uygular.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(KLMNDbContext).Assembly);

        modelBuilder.ApplyGlobalQueryFilters();
    }
}
