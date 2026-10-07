using KLMN.Api.Infrastructure;
using KLMN.Application;
using KLMN.Infrastructure;
using KLMN.Persistence;
using KLMN.Persistence.Extensions;

const string AngularCorsPolicy =
    "AngularCors";

var builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApplication(
    builder.Configuration);

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddPersistence(
    builder.Configuration);

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

builder.Services.AddOpenApi();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            AngularCorsPolicy,
            policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
    });

var app =
    builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi()
        .AllowAnonymous();

    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                "/openapi/v1.json",
                "KLMN API v1");
        });
}

await app.Services
    .InitializeDatabaseAsync();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(
    AngularCorsPolicy);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
