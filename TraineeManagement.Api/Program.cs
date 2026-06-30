using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Services.HealthCheckServices;
using TraineeManagement.Api.Data;
using System.Text.Json.Serialization;
using TraineeManagement.Api.Settings;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Swashbuckle.AspNetCore.Swagger;
using System.ComponentModel;
using StackExchange.Redis;
using TraineeManagement.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

using DotNetEnv;
DotNetEnv.Env.Load();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:8080");
            policy.AllowCredentials();
            policy.WithHeaders("accept", "content-type", "origin", "Authorization");
        });
});

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage")
);

builder.Services.Configure<RedisSettings>(
    builder.Configuration.GetSection("Redis")
);

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMq")  
);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(logEvent =>
            logEvent.Properties.TryGetValue("SourceContext", out var value) &&
            (value.ToString().Contains("Microsoft") ||
            value.ToString().Contains("System")))
        .WriteTo.File(
            path: "logs/app-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}"))
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

builder.Services.AddControllers().AddJsonOptions(
    options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        )
    );

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<ITraineeService, TraineeServices>();
builder.Services.AddScoped<IUserService, UserServices>();
builder.Services.AddScoped<IMentorService, MentorServices>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskServices>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentServices>();
builder.Services.AddScoped<ISubmissionService, SubmissionServices>();
builder.Services.AddScoped<IReviewService, ReviewServices>();
builder.Services.AddScoped<ISubmissionFileService, SubmissionFileServices>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobServices>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageServices>();

builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IConnectionManagerService, ConnectionManagerService>();
builder.Services.AddSingleton<ISubmissionProcessingPublisherService, SubmissionProcessingPublisher>();

builder.Services.AddScoped<ITokenService, TokenService>();

// database
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// redis
builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.Configuration = builder.Configuration["Redis:ConnectionString"];
     options.InstanceName = builder.Configuration["Redis:InstanceName"];
 });

// string redisConnectionString = builder.Configuration["RedisSettings:ConnectionString"] ?? "localhost:6379";
string redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "trainee-management-redis:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Trainee_Management.Api",
        Version = "v1"

    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT Status Below"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement

    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []

    }
    );
});

builder.Services.AddHealthChecks()
    .AddCheck<MySqlHealthCheck>("mysql")
    .AddCheck<RedisHealthCheck>("redis")
    .AddCheck<RabbitMQHealthCheck>("rabbitMQ");

// var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();
 
    await context.Database.MigrateAsync();
 
    await DbSeeder.SeedAdminUserAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Trainee api v1");
    });
}

app.MapGet("/", () =>
{
    return "Hello!!!";
});

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
