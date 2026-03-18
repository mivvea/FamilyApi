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


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateAudience = false,
                   ValidateIssuer = false,
                   ValidateLifetime = false,
                   ValidateIssuerSigningKey = false,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"]))
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

builder.Services.AddOpenApi();
builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
