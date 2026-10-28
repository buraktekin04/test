using KLMN.Domain.Entities.Admissions;
using KLMN.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

static void Assert(bool ok, string description)
{
    if (!ok) throw new InvalidOperationException(description);
    Console.WriteLine("PASS: " + description);
}

// Yalnız EF model metadata denetimidir; DB sunucusuna bağlantı açılmaz.
var options = new DbContextOptionsBuilder<KLMNDbContext>()
    .UseNpgsql("Host=127.0.0.1;Database=metadata_only;Username=metadata_only;Password=metadata_only")
    .Options;
using var db = new KLMNDbContext(options);
var model = db.Model;

foreach (var type in new[] { typeof(ProcedureTypeDef), typeof(SYYDiagnosisTypeDef), typeof(SYYDiagnosisSliceDef) })
{
    var entity = model.FindEntityType(type)!;
    Assert(entity != null, type.Name + " EF mapping mevcut");
    var codeIndex = entity!.GetIndexes().Single(x => x.Properties.Count == 1 &&
        x.Properties[0].Name == "Code");
    Assert(codeIndex.IsUnique, type.Name + " Code benzersiz");
    Assert(codeIndex.GetFilter()?.Contains("IsDeleted", StringComparison.Ordinal) == true,
        type.Name + " soft-delete partial unique");
    Assert(entity.GetKeys().All(k => k.Properties.All(p => p.Name == "Id")),
        type.Name + " Code alternate key bulunmuyor");
}

void VerifyForeignKey(Type dependentType, string navigation, string idProperty)
{
    var dependent = model.FindEntityType(dependentType)!;
    var fk = dependent.GetForeignKeys().Single(x => x.DependentToPrincipal?.Name == navigation);
    Assert(fk.Properties.Count == 1 && fk.Properties[0].Name == idProperty,
        dependentType.Name + "." + navigation + " GUID ID üzerinden FK");
    Assert(fk.PrincipalKey.Properties.Single().Name == "Id",
        dependentType.Name + "." + navigation + " principal PK kullanıyor");
    Assert(fk.DeleteBehavior == DeleteBehavior.Restrict, navigation + " silinmeye karşı Restrict");
}
VerifyForeignKey(typeof(ProcedureDef), "TypeDefinition", "ProcedureTypeDefId");
VerifyForeignKey(typeof(SYYDiagnosisDef), "DiagnosisTypeDef", "SYYDiagnosisTypeDefId");
VerifyForeignKey(typeof(SYYDiagnosisDef), "SliceDef", "SYYDiagnosisSliceDefId");

Console.WriteLine("EF model regresyonu tamamlandı. Gerçek PostgreSQL migration/CRUD testleri ayrıca gereklidir.");
