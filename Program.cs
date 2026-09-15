using AutoAlertBackEnd;
using AutoAlertBackEnd.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using AutoAlertBackEnd.Context;
using AutoAlertBackEnd.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExternal(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "AutoAlerBack", Version = "v1" });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyMethod()  // Permitir cualquier metodo HTTP
            .AllowAnyHeader()
            .AllowCredentials()); // Permitir cualquier cabecera
});

// Validate JWT config early to fail fast and avoid nullable warnings
var jwtKey = builder.Configuration["Jwt:Key"];
var google = builder.Configuration.GetSection("Authentication:Google");

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Configuration value 'Jwt:Key' is missing or empty. Please set it in appsettings.");
}

var key = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Configurar políticas de autorización desde archivo externo
builder.Services.AddAuthorization(AuthorizationPolicies.ConfigurePolicies);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});
var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Seed:Enabled"))
{
    using var scope = app.Services.CreateScope();
    await DatabaseSeeder.SeedAsync(
        scope.ServiceProvider.GetRequiredService<AutoAlertContext>(),
        scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder"));
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthUsePkce();
    });
}

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(
            new { message = "Acceso no autorizado, verifique sus credenciales." }
        );
        await context.Response.WriteAsync(result);
    }
});
app.UseAuthorization();
app.MapControllers();
app.Run();
