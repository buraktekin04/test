using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using KLMN.Application.Common.Authorization;
using KLMN.Application.Common.Interfaces.Authorization;
using KLMN.Application.Common.Interfaces.Identity;
using KLMN.Application.Common.Interfaces.Persistence;
using KLMN.Application.Users.Commands.CreateUser;
using KLMN.Application.Users.Commands.UpdateUser;
using KLMN.Application.Users.Exceptions;
using KLMN.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

// Handler-level tests use an async LINQ test double; PostgreSQL transactions/xmin
// and HTTP authorization middleware require separate integration testing.
static void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    Console.WriteLine("PASS: " + name);
}
static async Task Reject<T>(Func<Task> action, string name) where T : Exception
{
    try { await action(); } catch (T) { Check(true, name); return; }
    throw new Exception("Expected " + typeof(T).Name + ": " + name);
}
var create = new CreateUserCommand { UserName = "test", FirstName = "Test", LastName = "User",
    Email = "test@example.com", Password = "Test12345" };
var validator = new CreateUserCommandValidator();
Check(validator.Validate(create).IsValid, "Legacy create may omit role IDs");
Check(!validator.Validate(create with { RoleIds = [] }).IsValid, "Empty role selection rejected");
Check(!validator.Validate(create with { RoleIds = [Guid.Empty] }).IsValid, "Empty GUID rejected");
var roleId = Guid.NewGuid();
Check(!validator.Validate(create with { RoleIds = [roleId, roleId] }).IsValid, "Duplicate roles rejected");
Check(!validator.Validate(create with { RoleIds = Enumerable.Range(0, 101).Select(_ => Guid.NewGuid()).ToArray() }).IsValid, "Role selection bounded");

var f = new Fixture();
await f.Create(create);
Check(f.Links.Rows.Single().RoleId == f.Standard.Id && f.Db.Saves == 1, "Default standard role and user saved together");
f = new Fixture();
await f.Create(create with { RoleIds = [f.Standard.Id, f.Editor.Id] });
Check(f.Links.Rows.Count == 2 && f.Db.Saves == 1, "Explicit multiple roles saved together");
f = new Fixture { Allowed = false };
await Reject<UserRoleAssignmentForbiddenException>(() => f.Create(create with { RoleIds = [f.Editor.Id] }), "Unauthorized create role injection rejected");
Check(f.Db.Saves == 0, "Rejected assignment never saved");
f = new Fixture { Admin = false };
await Reject<UserRoleAssignmentForbiddenException>(() => f.Create(create with { RoleIds = [f.AdminRole.Id] }), "Delegated role assigner cannot grant ADMIN");
f = new Fixture();
await Reject<UserRoleAssignmentForbiddenException>(() => f.Create(create with { RoleIds = [f.AdminRole.Id] }), "Even ADMIN cannot grant ADMIN to a new account");
Check(f.Db.Saves == 0, "ADMIN grant rejection does not persist a new account");
f = new Fixture();
var userWithStandardRole = f.Existing();
await Reject<UserRoleAssignmentForbiddenException>(
    () => f.Update(userWithStandardRole, [f.AdminRole.Id]),
    "Even ADMIN cannot promote an existing account to ADMIN");
f = new Fixture();
f.Editor.IsActive = false;
await Reject<UserConflictException>(() => f.Create(create with { RoleIds = [f.Editor.Id] }), "Inactive new role rejected");
f = new Fixture();
await Reject<UserConflictException>(() => f.Create(create with { RoleIds = [Guid.NewGuid()] }), "Unknown role rejected");
f = new Fixture();
var user = f.Existing();
var stamp = user.SecurityStamp;
await f.Update(user, [f.Editor.Id]);
Check(!f.Links.Rows.Single(x => x.RoleId == f.Standard.Id).IsActive && f.Links.Rows.Single(x => x.RoleId == f.Editor.Id).IsActive,
    "Update replaces effective memberships");
Check(stamp != user.SecurityStamp && f.Db.Saves == 1, "Role-only update rotates security stamp in single save");
await f.Update(user, [f.Standard.Id]);
Check(f.Links.Rows.Count == 2 && f.Links.Rows.Single(x => x.RoleId == f.Standard.Id).IsActive, "Reassignment reuses inactive link without duplicate key");
f = new Fixture { Allowed = false };
user = f.Existing(); stamp = user.SecurityStamp;
await f.Update(user, null);
Check(f.Links.Rows.Single().IsActive && user.SecurityStamp == stamp, "Profile update without role permission preserves roles");
await Reject<UserRoleAssignmentForbiddenException>(() => f.Update(user, [f.Editor.Id]), "Unauthorized update role injection rejected");
f = new Fixture(); user = f.Existing(); f.Actor.Id = user.Id;
await Reject<UserRoleAssignmentForbiddenException>(() => f.Update(user, [f.Editor.Id]), "Self role changes rejected");
await f.Update(user, [f.Standard.Id]);
Check(f.Db.Saves == 1, "Unchanged own roles allow profile update");
f = new Fixture(); user = f.Existing();
await Reject<DbUpdateConcurrencyException>(() => f.Update(user, [f.Editor.Id], 2), "Stale version rejected before role changes");
Check(f.Links.Rows.Count == 1 && f.Db.Saves == 0, "Concurrency conflict preserves memberships");
f = new Fixture { Admin = false }; user = f.Existing(f.AdminRole);
await Reject<UserRoleAssignmentForbiddenException>(() => f.Update(user, [f.Standard.Id]), "Delegated assigner cannot remove ADMIN");
f = new Fixture(); user = f.Existing(f.AdminRole);
await Reject<UserRoleAssignmentForbiddenException>(
    () => f.Update(user, [f.Standard.Id]), "Last active ADMIN cannot be removed");
Check(f.Links.Rows.Single().IsActive && f.Db.Saves == 0, "Last ADMIN rejection preserves role membership");
f = new Fixture();
user = f.Existing(f.AdminRole);
var remainingAdmin = f.Existing(f.AdminRole);
await f.Update(user, [f.Standard.Id]);
Check(f.Links.Rows.Any(x => x.UserId == user.Id && x.RoleId == f.AdminRole.Id && !x.IsActive) &&
      f.Links.Rows.Any(x => x.UserId == remainingAdmin.Id && x.RoleId == f.AdminRole.Id && x.IsActive),
      "An existing extra ADMIN can be removed while another ADMIN remains");
f = new Fixture();
var activeRootAdmin = f.Existing(f.AdminRole);
user = f.Existing(f.AdminRole);
user.IsActive = false;
await f.Update(user, [f.Standard.Id]);
Check(f.Links.Rows.Any(x => x.UserId == activeRootAdmin.Id && x.RoleId == f.AdminRole.Id && x.IsActive) &&
      f.Links.Rows.Any(x => x.UserId == user.Id && x.RoleId == f.AdminRole.Id && !x.IsActive),
      "Inactive legacy ADMIN can be cleaned up without removing the last active administrator");
f = new Fixture(); user = f.Existing(f.Editor); f.Editor.IsActive = false;
await f.Update(user, [f.Editor.Id, f.Standard.Id]);
Check(f.Links.Rows.Count == 2, "Existing inactive role preserved while adding active role");
Console.WriteLine("All user-role regression checks passed.");

sealed class Fixture : IPermissionChecker, IUserAuthorizationService
{
    public bool Allowed = true;
    public bool Admin = true;
    public Actor Actor = new();
    public Role Standard = new() { Code = "STANDARD_USER", Name = "Standart" };
    public Role Editor = new() { Code = "EDITOR", Name = "Editor" };
    public Role AdminRole = new() { Code = "ADMIN", Name = "Admin" };
    public MemorySet<User> Users = new();
    public MemorySet<UserRole> Links = new();
    public DbProxy Db;
    private readonly IKLMNDbContext context;
    public Fixture()
    {
        context = DispatchProxy.Create<IKLMNDbContext, DbProxy>();
        Db = (DbProxy)context;
        Db.Sets["Users"] = Users;
        Db.Sets["UserRoles"] = Links;
        Db.Sets["Roles"] = new MemorySet<Role>([Standard, Editor, AdminRole]);
    }
    public User Existing(Role? role = null)
    {
        var user = new User { UserName = "test", NormalizedUserName = "TEST", FirstName = "Test", LastName = "User",
            Email = "test@example.com", NormalizedEmail = "TEST@EXAMPLE.COM", Version = 1 };
        Users.Rows.Add(user); role ??= Standard;
        Links.Rows.Add(new UserRole { UserId = user.Id, RoleId = role.Id, Role = role, User = user });
        return user;
    }
    public Task Create(CreateUserCommand command) => new CreateUserCommandHandler(context, new Hasher(), TimeProvider.System, Actor, this, this)
        .Handle(command, default);
    public Task Update(User user, Guid[]? ids, uint version = 1) => new UpdateUserCommandHandler(context, Actor, this, this)
        .Handle(new UpdateUserCommand { Id = user.Id, Version = version, UserName = user.UserName,
            FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, RoleIds = ids }, default);
    public Task<bool> HasPermissionAsync(string code, CancellationToken ct = default) => Task.FromResult(Allowed);
    public Task<UserAuthorizationSnapshot> GetAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(new UserAuthorizationSnapshot { IsAdmin = Admin, Roles = [], Permissions = [] });
}
public class DbProxy : DispatchProxy
{
    public Dictionary<string, object> Sets = new();
    public int Saves;
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        if (method!.Name == "SaveChangesAsync") { Saves++; return Task.FromResult(1); }
        return Sets[method.Name[4..]];
    }
}
sealed class Actor : ICurrentUserService
{
    public Guid Id = Guid.NewGuid();
    public Guid? UserId => Id;
    public string? UserName => "actor";
    public string? Email => null;
    public string? FirstName => null;
    public string? LastName => null;
    public string? FullName => null;
    public Guid? MilitaryUnitDefId => null;
    public IReadOnlyCollection<string> Roles => [];
    public IReadOnlyCollection<string> Permissions => [];
    public bool IsAuthenticated => true;
    public string? IpAddress => null;
    public string? UserAgent => null;
}
sealed class Hasher : IPasswordHasherService
{
    public string HashPassword(string password) => "test-double-hash";
    public bool VerifyPassword(string hash, string password) => false;
}
sealed class MemorySet<T>(IEnumerable<T>? seed = null) : DbSet<T>, IQueryable<T>, IAsyncEnumerable<T> where T : class
{
    public override Microsoft.EntityFrameworkCore.Metadata.IEntityType EntityType => throw new NotSupportedException();
    public List<T> Rows { get; } = seed?.ToList() ?? [];
    private IQueryable<T> Query => new AsyncRows<T>(Rows);
    Type IQueryable.ElementType => typeof(T);
    Expression IQueryable.Expression => Query.Expression;
    IQueryProvider IQueryable.Provider => Query.Provider;
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => Rows.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Rows.GetEnumerator();
    public override IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken ct = default) => new AsyncIterator<T>(Rows.GetEnumerator());
    public override ValueTask<EntityEntry<T>> AddAsync(T entity, CancellationToken ct = default)
    { Rows.Add(entity); return new ValueTask<EntityEntry<T>>((EntityEntry<T>)null!); }
}
sealed class AsyncRows<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public AsyncRows(IEnumerable<T> rows) : base(rows) { }
    public AsyncRows(Expression expression) : base(expression) { }
    IQueryProvider IQueryable.Provider => new AsyncProvider(this);
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken ct = default) => new AsyncIterator<T>(this.AsEnumerable().GetEnumerator());
}
sealed class AsyncIterator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    public T Current => inner.Current;
    public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(inner.MoveNext());
    public ValueTask DisposeAsync() { inner.Dispose(); return ValueTask.CompletedTask; }
}
sealed class AsyncProvider(IQueryProvider inner) : IAsyncQueryProvider
{
    public IQueryable CreateQuery(Expression expression) => throw new NotSupportedException();
    public IQueryable<T> CreateQuery<T>(Expression expression) => new AsyncRows<T>(expression);
    public object? Execute(Expression expression) => inner.Execute(expression);
    public T Execute<T>(Expression expression) => inner.Execute<T>(expression);
    public T ExecuteAsync<T>(Expression expression, CancellationToken ct = default)
    {
        var resultType = typeof(T).GetGenericArguments()[0];
        var value = typeof(IQueryProvider).GetMethods().Single(m => m.Name == "Execute" && m.IsGenericMethod)
            .MakeGenericMethod(resultType).Invoke(inner, [expression]);
        return (T)typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(resultType).Invoke(null, [value])!;
    }
}
