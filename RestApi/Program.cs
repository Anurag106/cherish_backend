using Cherish.RestApi.Services;
using Domain.Providers;
using Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Provider;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add API Versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
});

// Add the versioned API explorer (needed for Swagger per-version)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g. v1, v1.0
    options.SubstituteApiVersionInUrl = true; // substitutes {version:apiVersion} in route templates
});

// Configure Swagger to emit a document per API version
builder.Services.AddSwaggerGen(options =>
{
    // Basic JWT support in Swagger UI (optional)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });

    // You can further customize here; per-version docs are added at runtime using IApiVersionDescriptionProvider
});
// Configure CORS from appsettings.json
var corsSettings = builder.Configuration.GetSection("Cors");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", policy =>
    {
        policy.WithOrigins(corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .WithMethods(corsSettings.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" })
              .WithHeaders(corsSettings.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "Content-Type", "Authorization", "Accept", "Origin", "X-Requested-With" });

        if (corsSettings.GetValue<bool>("AllowCredentials"))
        {
            policy.AllowCredentials();
        }
    });
});

// Configure PostgreSQL connection string
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

// Configure NpgsqlDataSource with EnableDynamicJson for JSON/JSONB support
var connectionString = DatabaseConnectionManager.GetConnectionString(builder.Configuration);
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();
builder.Services.AddSingleton(dataSourceBuilder.Build());

// Register services
builder.Services.AddScoped<IUserProvider, PostgreSQLUserProvider>();
builder.Services.AddScoped<ICompanyProvider, PostgreSQLCompanyProvider>();
builder.Services.AddScoped<ITeamProvider, PostgreSQLTeamProvider>();
builder.Services.AddScoped<IHashtagProvider, PostgreSQLHashtagProvider>();
builder.Services.AddScoped<ITransactionProvider, PostgreSQLTransactionProvider>();
builder.Services.AddScoped<IPostProvider, PostgreSQLPostProvider>();
builder.Services.AddScoped<ICommentProvider, PostgreSQLCommentProvider>();
builder.Services.AddScoped<IReactionProvider, PostgreSQLReactionProvider>();
builder.Services.AddScoped<IFollowProvider, PostgreSQLFollowProvider>();
builder.Services.AddScoped<IAnalyticsProvider, MockAnalyticsProvider>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IHashtagService, HashtagService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IContentParsingService, ContentParsingService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "Cherish",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "Cherish",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Enable CORS
app.UseCors("AllowConfiguredOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class ConnectionStrings
{
    public string PostgreSQL { get; set; } = string.Empty;
}
