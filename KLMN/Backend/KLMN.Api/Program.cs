using KLMN.Api.ExceptionHandling;
using KLMN.Application;
using KLMN.Infrastructure;
using KLMN.Persistence;
using KLMN.Persistence.Extensions;

// Kimlik doğrulama gerektiren Angular isteklerine uygulanacak CORS politikasının adıdır.
const string AngularCorsPolicy = "AngularCors";

// ASP.NET Core hizmetlerinin ve yapılandırmanın kaydedildiği uygulama oluşturucusudur.
var builder = WebApplication.CreateBuilder(args);

/*
 * KLMN'nin bütün uygulama ayarları tek dosyada tutulur:
 * KLMN.Api/appsettings.json
 *
 * Bu dosyayı varsayılan configuration provider'larından sonra yeniden
 * eklemek, aynı anahtarın başka bir provider tarafından (örneğin
 * development ortamındaki eski ayarlar veya user-secrets) yanlışlıkla
 * override edilmesini engeller. Appsettings bütün KLMN ayarlarında önceliklidir.
 */
builder.Configuration.AddJsonFile(
    "appsettings.json",
    optional: false,
    reloadOnChange: true);

/*
 * İzin verilen Angular origin'leri sabit kod yerine appsettings.json
 * dosyasındaki Cors:AllowedOrigins bölümünden okunur.
 *
 * Angular tarafında proxy kullanılmadığı için frontend HTTPS adresinin
 * burada tanımlanmış olması gerekir.
 */
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (allowedOrigins is null ||
    allowedOrigins.Length == 0 ||
    allowedOrigins.Any(string.IsNullOrWhiteSpace))
{
    throw new InvalidOperationException(
        "appsettings.json içerisindeki Cors:AllowedOrigins bölümüne en az bir geçerli origin ekleyiniz.");
}

builder.Services.AddControllers();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddPersistence(
    builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        AngularCorsPolicy,
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Middleware ve endpointlerin tanımlandığı çalıştırılabilir ASP.NET Core uygulamasıdır.
var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "KLMN API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors(AngularCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
