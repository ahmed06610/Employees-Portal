using EmployeesPortal.Data;
using Hangfire;
using Hangfire.SqlServer;
using Vacation_Service.Interfaces;
using Vacation_Service.servicerRR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vacation_Service.Repositories;
using Vacation_Service.Services;

namespace Vacation_Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            

            // Register Hangfire services
            builder.Services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                             .UseSimpleAssemblyNameTypeSerializer()
                             .UseRecommendedSerializerSettings()
                             .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
                             {
                                 CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                 QueuePollInterval = TimeSpan.Zero,
                                 UseRecommendedIsolationLevel = true,
                                 DisableGlobalLocks = true
                             }));

            // Add the Hangfire server
            builder.Services.AddHangfireServer();

            // Register Vacation Services and other services
            builder.Services.AddScoped<IVacationService, VacationService>();
            builder.Services.AddScoped<IVacationRepository, VacationRepository>();
            builder.Services.AddScoped<VacationEscalationJob>();
            builder.Services.AddScoped<IRabbitMqIntgration, RabbitMqIntegration>();


            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"]
                };
            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Configure Hangfire dashboard
            app.UseHangfireDashboard("/dashboard");

            // Schedule the recurring Hangfire job for the vacation escalation process
            RecurringJob.AddOrUpdate<VacationEscalationJob>(
                "escalate-vacation-requests",
                job => job.ExecuteAsync(),
                Cron.DayInterval(1));  // This schedules the job to run every 2 days


            app.MapControllers();

            app.Run();
        }
    }
}
