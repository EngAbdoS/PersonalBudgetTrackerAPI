using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PersonalBudgetTrackerAPI;
using PersonalBudgetTrackerAPI.Authorization.Handlers;
using PersonalBudgetTrackerAPI.Authorization.Policies;
using PersonalBudgetTrackerAPI.Authorization.Requirements;
using PersonalBudgetTrackerAPI.BackgroundJobs.Implementations;
using PersonalBudgetTrackerAPI.BackgroundJobs.Interfaces;
using PersonalBudgetTrackerAPI.BackgroundJobs.Schedulers;
using PersonalBudgetTrackerAPI.DatabaseContext;
using PersonalBudgetTrackerAPI.Identity;
using PersonalBudgetTrackerAPI.Middleware;
using PersonalBudgetTrackerAPI.MongoDB.Settings;
using PersonalBudgetTrackerAPI.Services.Implementations;
using PersonalBudgetTrackerAPI.Services.Interfaces;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!)
);
builder.Services.Configure<MongoDbSettings>( builder.Configuration.GetSection("MongoDB"));
BsonSerializer.RegisterSerializer(new DateOnlySerializer());

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(
        builder.Configuration["MongoDB:ConnectionString"]));


builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHangfireServer();



builder.Services.AddScoped<ITokenStore, RedisTokenStore>();
builder.Services.AddScoped<IDaySnapshotService, RedisDaySnapshotService>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthorizationHandler, DbRoleHandler>();
// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
     options =>
     {
         options.Password.RequiredLength = 6;
         options.Password.RequireDigit = true;
         options.Password.RequireNonAlphanumeric = true;
         options.Password.RequireUppercase = true;
         options.Password.RequireLowercase = true;
     })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(PolicyNames.AdminFromDb, policy =>
    {
        /*
        good when access token have long expiration time 
        useless with short lived access token with refresh token 
        */
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new DbRoleRequirement("Admin"));
    });

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddScoped<IReasonService, ReasonService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
builder.Services.AddScoped<ITransactionPartnerService, TransactionPartnerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPendingTransactionCacheService, PendingTransactionCacheService>();

builder.Services.AddScoped<IFinanialRuleService, FinanialRuleService>();
builder.Services.AddScoped<IDailySnapshotMongoService, DailySnapshotMongoService>();
builder.Services.AddScoped<ISnapshotPromotionJob, SnapshotPromotionJob>();
builder.Services.AddScoped<IFinancialAggregatorService, FinancialAggregatorService>();
builder.Services.AddScoped<ITransactionValidationService, TransactionValidationService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Personal Budget Tracker API");
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
     SeedData.Seed(services);
}

SnapshotPromotionJobScheduler.Register(app.Services);

app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();


app.UseMiddleware<ExceptionMiddleware>();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();

