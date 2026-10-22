using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace KLMN.Persistence.Configurations.Admissions;
/// <summary>Sayısal Slice kodlarını korur; A/B/C/D değerlerini migration seed olarak ekler.</summary>
public sealed class SYYDiagnosisSliceDefConfiguration:BaseEntityConfiguration<SYYDiagnosisSliceDef>
{
    protected override void ConfigureEntity(EntityTypeBuilder<SYYDiagnosisSliceDef> b)
    {
        b.ToTable("SYYDiagnosisSliceDefs");
        b.Property(x=>x.Name).HasMaxLength(150).IsRequired();
        b.Property(x=>x.Code).ValueGeneratedNever();
        b.HasAlternateKey(x=>x.Code);
        // Kaynak fotoğraf: 1=A, 2=B, 3=C, 4=D. GUID'ler migrationlar boyunca değişmez.
        b.HasData(
          new {Id=Guid.Parse("267733b0-a50c-41c9-b116-0e6cfef14a01"),Code=1,Name="A",
            CreatedDate=new DateTime(2026,10,22,0,0,0,DateTimeKind.Utc),IsActive=true,IsDeleted=false},
          new {Id=Guid.Parse("267733b0-a50c-41c9-b116-0e6cfef14a02"),Code=2,Name="B",
            CreatedDate=new DateTime(2026,10,22,0,0,0,DateTimeKind.Utc),IsActive=true,IsDeleted=false},
          new {Id=Guid.Parse("267733b0-a50c-41c9-b116-0e6cfef14a03"),Code=3,Name="C",
            CreatedDate=new DateTime(2026,10,22,0,0,0,DateTimeKind.Utc),IsActive=true,IsDeleted=false},
          new {Id=Guid.Parse("267733b0-a50c-41c9-b116-0e6cfef14a04"),Code=4,Name="D",
            CreatedDate=new DateTime(2026,10,22,0,0,0,DateTimeKind.Utc),IsActive=true,IsDeleted=false});
    }
}
