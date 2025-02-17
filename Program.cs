// Program.cs
using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ExchangeRateService>().AddStandardResilienceHandler();

//builder.Services.AddResiliencePipeline("default", x =>
//{
//    x.AddRetry(new RetryStrategyOptions
//    {
//        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
//        MaxRetryAttempts = 3,
//        Delay = TimeSpan.FromSeconds(2),
//        BackoffType = DelayBackoffType.Exponential,
//        UseJitter = true
//    })
//    .AddTimeout(TimeSpan.FromSeconds(10));
//});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourissuer",
            ValidAudience = "youraudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

//builder.Services.AddOpenTelemetry(builder => 
//{
//    builder.AddAspNetCoreInstrumentation()
//           .AddHttpClientInstrumentation()
//           .AddJaegerExporter();
//});

app.MapControllers();

app.Run();

public partial class Program
{
}