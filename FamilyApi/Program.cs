using FamilyApi;
using FamilyApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>new MongoClient(builder.Configuration["AppSettings:MongoString"]));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("sample_mflix"));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();

var secretKey = builder.Configuration["AppSettings:SecretKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Configuration value 'AppSettings:SecretKey' is missing or empty.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateAudience = false,
                   ValidateIssuer = false,
                   ValidateLifetime = false,
                   ValidateIssuerSigningKey = false,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
               };
               options.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       // Log the exception during token validation failure
                       Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                       return Task.CompletedTask;
                   },
                   OnTokenValidated = context =>
                   {
                       // Log when the token is successfully validated
                       Console.WriteLine("Token validated successfully.");
                       return Task.CompletedTask;
                   },
                   OnChallenge = context =>
                   {
                       // Log the challenge event (this happens if the token is missing or invalid)
                       Console.WriteLine($"Challenge: {context.ErrorDescription}");
                       return Task.CompletedTask;
                   }
               };
           });

builder.Services.AddCors(options =>
{
    options.AddPolicy("FamilyApp", policy =>
    {
        policy
            .WithOrigins(
                "https://mivvea.github.io",
                "http://localhost:3000",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors("FamilyApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
