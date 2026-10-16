using KLMN.Application.MilitaryUnits.Commands.CreateMilitaryUnitDef;
using KLMN.Application.MilitaryUnits.Commands.UpdateMilitaryUnitDef;
using KLMN.Domain.Entities.Identity;
using KLMN.Domain.Entities.MilitaryUnit;
using KLMN.Persistence.Contexts;
using KLMN.Persistence.MigrationSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

static void Check(bool valid, string message)
{
    if (!valid) throw new InvalidOperationException(message);
    Console.WriteLine("PASS: " + message);
}

var start = new DateTime(2026, 10, 13, 9, 0, 0, DateTimeKind.Utc);
var create = new CreateMilitaryUnitDefCommand("Birlik", "BIRLIK_01", null, "EXT", null, start, start.AddDays(1));
var validator = new CreateMilitaryUnitDefCommandValidator();
Check(validator.Validate(create).IsValid, "UTC validity interval accepted");
Check(validator.Validate(create with { ValidFrom = null, ValidTo = null }).IsValid, "Unbounded interval accepted");
Check(!validator.Validate(create with { ValidTo = start }).IsValid, "Empty interval rejected");
Check(!validator.Validate(create with { ValidTo = start.AddDays(-1) }).IsValid, "Reversed interval rejected");
Check(!validator.Validate(create with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Unspecified) }).IsValid, "Non-UTC timestamp rejected");
Check(!validator.Validate(create with { ForcesCommandDef = Guid.Empty }).IsValid, "Empty force reference rejected");
var update = new UpdateMilitaryUnitDefCommand(Guid.NewGuid(), 1, "Birlik", "BIRLIK_01", null);
Check(!new UpdateMilitaryUnitDefCommandValidator().Validate(update with { Version = 0 }).IsValid, "Update requires concurrency version");

using var db = new KLMNDbContext(new DbContextOptionsBuilder<KLMNDbContext>()
    .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options);
var unit = db.Model.FindEntityType(typeof(MilitaryUnitDef))!;
Check(unit.GetTableName() == "MilitaryUnitDefs", "New table mapped");
Check(!db.Model.GetEntityTypes().Any(x => x.ClrType.Name == "OrganizationUnit"), "Old entity removed from EF model");
Check(unit.FindProperty("Version")!.IsConcurrencyToken && unit.FindProperty("Version")!.GetColumnName() == "xmin", "PostgreSQL xmin concurrency preserved");
var userFk = db.Model.FindEntityType(typeof(User))!.GetForeignKeys().Single(x => x.PrincipalEntityType == unit);
Check(userFk.Properties.Single().Name == "MilitaryUnitDefId" && userFk.DeleteBehavior == DeleteBehavior.Restrict, "User duty station FK is restrictive");
Check(unit.GetForeignKeys().Single(x => x.PrincipalEntityType == unit).Properties.Single().Name == "ConnectedTopUnity", "Parent hierarchy mapped explicitly");

foreach (var schema in new string?[] { null, "mskbs" })
{
    var up = new MigrationBuilder("Npgsql.EntityFrameworkCore.PostgreSQL");
    MilitaryUnitDefMigrationOperations.Up(up, schema);
    Check(!up.Operations.Any(x => x is DropTableOperation or CreateTableOperation or DropColumnOperation), "Upgrade preserves existing rows and columns: " + (schema ?? "default"));
    var sql = string.Join("\n", db.GetService<IMigrationsSqlGenerator>().Generate(up.Operations).Select(x => x.CommandText));
    Check(sql.Contains("RENAME TO \"MilitaryUnitDefs\"") && sql.Contains("RENAME COLUMN \"OrganizationUnitId\" TO \"MilitaryUnitDefId\""), "Npgsql emits table and FK-column renames");
    var down = new MigrationBuilder("Npgsql.EntityFrameworkCore.PostgreSQL");
    MilitaryUnitDefMigrationOperations.Down(down, schema);
    var rollback = string.Join("\n", db.GetService<IMigrationsSqlGenerator>().Generate(down.Operations).Select(x => x.CommandText));
    Check(rollback.Contains("RENAME TO \"OrganizationUnits\""), "Rollback SQL generated");
}
var forceCreate = new KLMN.Application.ForcesCommands.Commands.CreateForcesCommandDef.CreateForcesCommandDefCommand("Kara Kuvvetleri", "KKK", start, start.AddDays(1));
var forceValidator = new KLMN.Application.ForcesCommands.Commands.CreateForcesCommandDef.CreateForcesCommandDefCommandValidator();
Check(forceValidator.Validate(forceCreate).IsValid, "Force definition accepts valid UTC interval");
Check(forceValidator.Validate(forceCreate with { ValidFrom = null, ValidTo = null }).IsValid, "Force definition accepts open bounds");
Check(!forceValidator.Validate(forceCreate with { ValidTo = start }).IsValid, "Force definition rejects equal date bounds");
Check(!forceValidator.Validate(forceCreate with { Name = " " }).IsValid, "Force definition requires a name");
Check(!forceValidator.Validate(forceCreate with { Code = "bad code" }).IsValid, "Force definition rejects malformed codes");
Check(!forceValidator.Validate(forceCreate with { ValidFrom = DateTime.SpecifyKind(start, DateTimeKind.Unspecified) }).IsValid, "Force definition rejects non-UTC dates");
var forceType = db.Model.FindEntityType(typeof(ForcesCommandDef))!;
Check(forceType.GetTableName() == "ForcesCommandDefs", "Force definition table mapped");
Check(forceType.FindProperty("Version")!.IsConcurrencyToken && forceType.FindProperty("Version")!.GetColumnName() == "xmin", "Force definition uses xmin concurrency");
var forceFk = unit.GetForeignKeys().Single(x => x.PrincipalEntityType == forceType);
Check(forceFk.Properties.Single().Name == "ForcesCommandDef" && forceFk.DeleteBehavior == DeleteBehavior.Restrict && !forceFk.IsRequired, "Existing nullable force GUID is the restrictive FK without a shadow column");
Check(!unit.GetProperties().Any(x => x.IsShadowProperty() && x.ClrType == typeof(Guid?)), "Force relationship introduces no shadow GUID");
foreach (var schema in new string?[] { null, "mskbs" })
{
    var forceMigration = new MigrationBuilder("Npgsql.EntityFrameworkCore.PostgreSQL");
    ForcesCommandDefMigrationOperations.Up(forceMigration, schema);
    Check(!forceMigration.Operations.Any(x => x is DropTableOperation or DropColumnOperation or RenameColumnOperation), "Force migration preserves existing military columns");
    var forceSql = string.Join("\n", db.GetService<IMigrationsSqlGenerator>().Generate(forceMigration.Operations).Select(x => x.CommandText));
    Check(forceSql.Contains("FOREIGN KEY (\"ForcesCommandDef\")") && forceSql.Contains("ON DELETE RESTRICT"), "Force migration emits restrictive FK");
}
// Kuvvet ve üst birlik FK filtreleri için parametre doğrulama.
var forceFilterId = Guid.NewGuid();
var parentFilterId = Guid.NewGuid();
var filteredQuery = new KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs.GetMilitaryUnitDefsQuery(
    ForcesCommandDef: forceFilterId, ConnectedTopUnity: parentFilterId);
var filterValidator = new KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs.GetMilitaryUnitDefsQueryValidator();
Check(filterValidator.Validate(filteredQuery).IsValid, "Force and parent GUID filters accepted");
Check(!filterValidator.Validate(filteredQuery with { ForcesCommandDef = Guid.Empty }).IsValid,
    "Force filter rejects empty GUID");
Check(!filterValidator.Validate(filteredQuery with { ConnectedTopUnity = Guid.Empty }).IsValid,
    "Parent filter rejects empty GUID");
var filterSql = db.MilitaryUnitDefs.IgnoreQueryFilters().Where(x =>
    x.ForcesCommandDef == forceFilterId && x.ConnectedTopUnity == parentFilterId).ToQueryString();
Check(filterSql.Contains("\"ForcesCommandDef\"") && filterSql.Contains("\"ConnectedTopUnity\""),
    "Both FK filters translate into PostgreSQL SQL");

var projectedSql = db.MilitaryUnitDefs.IgnoreQueryFilters().Select(KLMN.Application.MilitaryUnits.Responses.MilitaryUnitDefResponse.Projection).ToQueryString();
Check(projectedSql.Contains("ForcesCommandDefs"), "Military list projects force labels in SQL");
// Ek bir FilterOptionsQuery oluşturmadan mevcut dropdown sorgularını yeniden kullan.
var defaultForces = new KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions.GetForceCommandOptionsQuery();
var allForces = new KLMN.Application.MilitaryUnits.Queries.GetForceCommandOptions.GetForceCommandOptionsQuery(true);
var defaultParents = new KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions.GetMilitaryUnitParentOptionsQuery();
var allParents = new KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitParentOptions.GetMilitaryUnitParentOptionsQuery(true);
Check(!defaultForces.IncludeInactive && !defaultParents.IncludeInactive,
    "Existing form dropdown queries default to active records");
Check(allForces.IncludeInactive && allParents.IncludeInactive,
    "Same dropdown queries accept inactive records for column filters");
var parentOption = new KLMN.Application.MilitaryUnits.Responses.MilitaryUnitParentOptionResponse(
    Guid.NewGuid(), "BIRLIK", "Birlik", null, "Kara Kuvvetleri");
Check(parentOption.ForceName == "Kara Kuvvetleri",
    "Existing parent option response can show force name in column filters");

Console.WriteLine("All regression checks passed. No database connection was opened.");
