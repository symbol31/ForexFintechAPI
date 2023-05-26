using FluentValidation;
using FluentValidation.AspNetCore;
using ForexFintechAPI.Action_Filters;
using ForexFintechAPI.Data;
using ForexFintechAPI.Models;
using ForexFintechAPI.Services;
using Hellang.Middleware.ProblemDetails;
using IdentityAPI.Services;
using IPayServiceReference;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers().AddFluentValidation(options =>
{
    options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
var configurationstring = builder.Configuration["ConnectionStrings:Default"];
builder.Services.AddDbContext<MyContext>(x => x.UseSqlServer(configurationstring));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
}).AddEntityFrameworkStores<MyContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuerSigningKey = true,
    };
});
builder.Services.AddScoped<iRemitIntWsSendServiceV4SoapClient>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IForexService, ForexService>();
builder.Services.AddHttpClient();
builder.Services.Configure<XEAPIConfiguration>(builder.Configuration.GetSection("XEAPIConfiguration"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authentication Techniques in SwaggerUI(\"bearer{token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanAccessExchangeRate", policy =>
    {
        policy.RequireAuthenticatedUser()
              .RequireRole("Admin");
    });
});
builder.Services.AddProblemDetails();
builder.Services.AddScoped<RateLimitingMiddleware>();
//builder.Services.AddScoped<ExceptionHandlerMiddleware>();
builder.Services.AddScoped<IValidator<PartnerDto>, PartnerDtoValidator>();
builder.Services.AddScoped<IValidator<ExchangeManipulationDataDto>, ExchangeManipulationDtoValidator>();
builder.Services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
builder.Services.AddScoped<ValidationFilter>();
var serviceDescriptors = builder.Services
    .Where(descriptor => typeof(IValidator) != descriptor.ServiceType
           && typeof(IValidator).IsAssignableFrom(descriptor.ServiceType)
           && descriptor.ServiceType.IsInterface)
    .ToList();

foreach (var descriptor in serviceDescriptors)
{
    builder.Services.Add(new ServiceDescriptor(
        typeof(IValidator),
        p => p.GetRequiredService(descriptor.ServiceType),
        descriptor.Lifetime));
}
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
configureLogging();
builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RateLimitingMiddleware>();
app.UseProblemDetails();
//app.UseMiddleware<ExceptionHandlerMiddleware>();

app.ExceptionHandling();

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

void configureLogging()
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(
        $"appsettings.{environment}.json", optional: true
        ).Build();
    Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .Enrich.WithExceptionDetails()
                  .WriteTo.Console()
                  .WriteTo.Elasticsearch(ConfigureElasticSink(configuration,environment))
                  .Enrich.WithProperty("Environment",environment)
                  .ReadFrom.Configuration(configuration)
                  .CreateLogger();
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat=$"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".","-")}-{environment.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
        NumberOfReplicas = 1,
        NumberOfShards = 1,

    };
}