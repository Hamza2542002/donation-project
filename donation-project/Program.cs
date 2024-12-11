
using donation_project.Helpers;
using donation_project.models;
using donation_project.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace donation_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"), options => options.EnableRetryOnFailure(
                    maxRetryCount: 5
                    )));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

            //builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailConfiguration")); // in dev

            //for deploument

            builder.Services.Configure<EmailSetting>(options =>
            {
                options.Email = builder.Configuration["EmailConfiguration_Email"] ?? "";
                options.Password = builder.Configuration["EmailConfiguration_Password"] ?? "";
                options.Host = builder.Configuration["EmailConfiguration_Host"] ?? "";
                options.Port = int.TryParse(builder.Configuration["EmailConfiguration_Port"], out var port) ? port : 587; // Default port if not set
                options.EnableSsl = bool.TryParse(builder.Configuration["EmailConfiguration_EnableSsl"], out var enableSsl) && enableSsl; // Default to true if not set
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:IssureIP"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:AudienceIP"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"] ?? "wjhehsakdhjkashd22yewuiouioqjasdkm,"))
                };

            });


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAccountService, AccountService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
