using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PatiantMicroService.Domain.Contracts;
using PatiantMicroService.Persistence.Data.DbContexts;
using PatiantMicroService.Persistence.Repositories;
using PatiantMicroService.Services.Services;
using PatiantMicroService.ServicesAbstraction.Interfaces;
using PatiantMicroService.Web.CustomMiddleWares;
using PatiantMicroService.Web.Factories;
using System.Security.Claims;
using System.Text;

namespace PatiantMicroServiceWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region 🔹 Controllers + API Behavior

            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory =
                    ApiResponseFactory.GenerateApiValidationResponse;
            });

            #endregion

            #region 🔹 DbContext

            builder.Services.AddDbContext<PatientDbContext>(options => {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });

            });

            #endregion

            #region 🔹 FluentValidation

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            #endregion

            #region 🔹 Repositories + UnitOfWork

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IIdentityClient, IdentityClient>();
            builder.Services.AddScoped<IAppDbContext, AppDbContext>();

            #endregion

            #region 🔹 Identity Client (HTTP Communication)

            builder.Services.AddHttpClient<IIdentityClient, IdentityClient>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["Identity:Authority"]!);
            });

            #endregion

            #region 🔹 JWT Authentication

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
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
                    ValidAudience = builder.Configuration["JWTOptions:Audience"],

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)
                    ),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier,

                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion

            #region 🔹 Authorization Policies

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", p => p.RequireRole("Doctor"));
                options.AddPolicy("PatiantOnly", p => p.RequireRole("Patiant"));
                options.AddPolicy("AdminOrDoctor", p => p.RequireRole("Admin", "Doctor"));
                options.AddPolicy("AdminOrPatiant", p => p.RequireRole("Admin", "Patiant"));
            });

            #endregion

            #region 🔹 CORS

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(7248);
            });

            #endregion

            #region 🔹 Swagger

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #endregion

            #region 🔹 Build App

            var app = builder.Build();

            #endregion

            #region 🔹 Auto Migration

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<PatientDbContext>();
                await db.Database.MigrateAsync();
            }

            #endregion

            #region 🔹 Middleware Pipeline

            // Global Exception Handler
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            // Request Logging
            app.UseMiddleware<RequestLoggingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            #endregion

            await app.RunAsync();
        }
    }
}