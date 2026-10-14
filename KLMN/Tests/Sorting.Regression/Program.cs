using KLMN.Application.Common.Models;
using KLMN.Application.Users.Queries.GetUsers;
using KLMN.Application.Roles.Queries.GetRoles;
using KLMN.Application.MilitaryUnits.Queries.GetMilitaryUnitDefs;
using KLMN.Application.ForcesCommands.Queries.GetForcesCommandDefs;
using KLMN.Domain.Common;
using KLMN.Domain.Entities.Identity;
using KLMN.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

var checks = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    checks++;
}
using var db = new KLMNDbContext(new DbContextOptionsBuilder<KLMNDbContext>()
    .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options);
void CheckSql<T>(IQueryable<T> query, DefinitionSort<T> sorting, string[] fields) where T : BaseEntity
{
    foreach (var field in fields)
    foreach (var direction in new[] { 1, -1 })
    {
        // Generate actual Npgsql SQL without opening a database connection.
        var sql = sorting.Apply(query, field, direction).Skip(20).Take(20).ToQueryString();
        Check(sql.Contains("ORDER BY") && sql.Contains("LIMIT") && sql.Contains("OFFSET"), $"{typeof(T).Name}.{field}: SQL sorting and paging");
        Check(sql.IndexOf("ORDER BY", StringComparison.Ordinal) < sql.LastIndexOf("LIMIT", StringComparison.Ordinal), "Sorting precedes paging");
        if (direction == -1) Check(sql.Contains("DESC"), $"Descending SQL: {field}");
    }
}
CheckSql(db.Users.IgnoreQueryFilters().Where(x => !x.IsDeleted), GetUsersSorting.Definition,
    ["fullName", "userName", "email", "militaryUnit", "roles", "isActive", "clockLimit", "validFrom", "validTo", "createdDate"]);
CheckSql(db.Roles, GetRolesSorting.Definition,
    ["name", "code", "description", "isSystemRole", "isActive", "userCount", "permissionCount", "createdDate"]);
CheckSql(db.MilitaryUnitDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted), GetMilitaryUnitDefsSorting.Definition,
    ["name", "code", "externalCode", "forcesCommandName", "connectedTopUnitName", "isActive", "userCount", "childCount", "validFrom", "validTo", "createdDate"]);
CheckSql(db.ForcesCommandDefs.IgnoreQueryFilters().Where(x => !x.IsDeleted), GetForcesCommandDefsSorting.Definition,
    ["name", "code", "isActive", "militaryUnitCount", "validFrom", "validTo", "createdDate"]);
var rows = new[] {
    new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "B" },
    new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "A" },
    new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "A" }
}.AsQueryable();
var asc = GetRolesSorting.Definition.Apply(rows, "name", 1).ToArray();
Check(asc[0].Id.CompareTo(asc[1].Id) < 0 && asc[2].Name == "B", "Equal names have stable Id order");
Check(GetRolesSorting.Definition.Apply(rows, "name", -1).First().Name == "B", "Descending value order");
Check(GetRolesSorting.Definition.Apply(rows, "name", 1).Skip(1).Take(1).Single().Id == asc[1].Id, "Page two uses full sorted data");
Check(!new GetUsersQueryValidator().Validate(new GetUsersQuery(SortField: "PasswordHash")).IsValid, "User sorting whitelist enforced");
Check(!new GetRolesQueryValidator().Validate(new GetRolesQuery(SortOrder: 0)).IsValid, "Invalid direction rejected");
Check(!new GetMilitaryUnitDefsQueryValidator().Validate(new GetMilitaryUnitDefsQuery(SortField: "Name; DROP TABLE Users")).IsValid, "Raw SQL cannot be injected");
Check(!new GetForcesCommandDefsQueryValidator().Validate(new GetForcesCommandDefsQuery(SortField: null!)).IsValid, "Null field rejected");
Console.WriteLine($"PASS: {checks} sorting checks. Npgsql SQL generated without database connection.");
