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
Check(unit.GetForeignKeys().Single().Properties.Single().Name == "ConnectedTopUnity", "Parent hierarchy mapped explicitly");

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
Console.WriteLine("All regression checks passed. No database connection was opened.");
