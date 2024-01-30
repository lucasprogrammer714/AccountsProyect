using Accounts.DAO.Model;
using AccountsProyect.BE;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
IConfiguration configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<AccountsControlContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DB_Accounts")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins().AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddTransient<AccountsProyect.Security.TokenJwtGenerator>();



builder.Services.Configure<AudienceModel>(builder.Configuration.GetSection("Audience"));
var audienceConfig = builder.Configuration.GetSection("Audience");
var secret = audienceConfig["Secret"];
var signInKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = signInKey,
    ValidateIssuer = false,
    ValidIssuer = "Localhost",
    ValidateAudience = false,
    ValidAudience = "CuentasProyect",
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,
    RequireExpirationTime = true,
};
builder.Services
    .AddAuthentication(x =>
    {
    x.DefaultAuthenticateScheme = audienceConfig["Secret"];
    x.DefaultChallengeScheme = audienceConfig["Secret"];
    })
    .AddJwtBearer(audienceConfig["Secret"], x =>
    {

        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = tokenValidationParameters;
        x.IncludeErrorDetails = true;
    });




builder.Services.AddSwaggerGen(options =>
{
    var groupName = "v1";

    options.SwaggerDoc(groupName, new OpenApiInfo
    {

        Title = $"CuentaProyect.API {groupName}",
        Version = groupName,
        Description = "CuentaProyect Server",
        
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, inserte el JWT con Bearer dentro del campo",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        new string[] { }
                    }
                });
});
        

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("../swagger/v1/swagger.json", "AccountProyect.API v1"));


//app.UseCors();


app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
