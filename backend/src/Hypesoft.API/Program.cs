using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Hypesoft API", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT obtido no Keycloak: Bearer {seu token}"
    };
    options.AddSecurityDefinition("Bearer", jwtScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

var frontendOrigins = new[] { "http://localhost:3000", "http://localhost:5173" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Authority: endereco INTERNO (rede do docker-compose) usado pelo backend pra buscar
// as chaves publicas do Keycloak (JWKS) em /.well-known/openid-configuration.
var keycloakAuthority = builder.Configuration["Keycloak:Authority"] ?? "http://keycloak:8080/realms/hypesoft";

// Issuer: endereco EXTERNO que o Keycloak realmente grava no campo "iss" dos tokens,
// que e o endereco que o cliente (curl, frontend, Postman) usou pra fazer login (localhost:8080).
// Os dois precisam ser configurados separadamente: o backend acessa o Keycloak via
// rede interna do Docker ("keycloak"), mas quem loga acessa via "localhost" (porta publicada).
var keycloakIssuer = builder.Configuration["Keycloak:ValidIssuer"] ?? "http://localhost:8080/realms/hypesoft";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.RequireHttpsMetadata = false; // OK em dev; habilitar HTTPS em producao
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // TODO: restringir para o client id quando os clients/roles estiverem definitivos (Dia 2/3)
            ValidateIssuer = true,
            ValidIssuer = keycloakIssuer
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
