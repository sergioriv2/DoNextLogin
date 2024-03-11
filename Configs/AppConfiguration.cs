using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using ServerlessLogin.Data;
using ServerlessLogin.Filters;
using ServerlessLogin.Interfaces;
using ServerlessLogin.Models;
using ServerlessLogin.Repositories;
using ServerlessLogin.Seeders;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;


namespace ServerlessLogin.Configs
{
    public class AppConfiguration
    {
        private IServiceCollection Services { get; set; }
        private WebApplicationBuilder Builder { get; set; }

        private void ConfigureAuthentication()
        {
            // JWT Config
            Services.AddIdentity<IdentityUser, IdentityRole>(
                               options =>
                               {
                                   options.Password.RequireUppercase = true;
                                   options.Password.RequireLowercase = true;
                                   options.Password.RequireDigit = true;
                                   options.SignIn.RequireConfirmedEmail = true;
                               }
                           )
                       .AddEntityFrameworkStores<DataContext>()
                       .AddDefaultTokenProviders();

            Services.AddAuthentication(
                    x =>
                    {
                        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    }
                )
            .AddJwtBearer(
                    options =>
                    {
                        var Key = Encoding.UTF8.GetBytes(Builder.Configuration.GetValue<string>("JWT:AccessKey"));
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Key)
                        };

                        options.Events = new JwtBearerEvents()
                        {
                            OnChallenge = async context =>
                            {
                                if (!context.Response.HasStarted)
                                {
                                    context.Response.StatusCode = ((int)HttpStatusCode.Unauthorized);
                                    context.Response.ContentType = "application/json";

                                    var UnauthorizedExceptionDetails = new
                                    {
                                        Property = "Authorization Header",
                                        Constraints = new
                                        {
                                            InvalidJwt = "Invalid JWT Provided"
                                        }
                                    };

                                    await context.Response.WriteAsJsonAsync(new
                                    {
                                        statusCode = ((int)HttpStatusCode.Unauthorized),
                                        message = "Unauthorized",
                                        payload = new object(),
                                        errors = new List<object>() {
                                            UnauthorizedExceptionDetails
                                         }
                                    });
                                }
                                context.HandleResponse();
                            },

                            OnForbidden = async context =>
                            {
                                if (!context.Response.HasStarted)
                                {
                                    context.Response.StatusCode = ((int)HttpStatusCode.Forbidden);
                                    context.Response.ContentType = "application/json";


                                    await context.Response.WriteAsJsonAsync(new
                                    {
                                        statusCode = ((int)HttpStatusCode.Forbidden),
                                        message = "Forbidden",
                                        payload = new object(),
                                        errors = new List<string>() {
                                            "Forbidden"
                                         }
                                    });
                                }
                            }
                        };

                    }
                );

            Services.AddSingleton<IJwtManagerRepository, JwtManagerRepository>();
            Services.AddScoped<IUserRepository, UserRepository>();
        }

        private void ConfigureRepositories()
        {
            // Controllers Config
            Services.AddControllers(
                    options =>
                    {
                        options.Filters.Add<ApiResponseFilter>();
                        options.Filters.Add<ValidationFilter>();
                    }
                )
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Repositories
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IGoogleAuthRepository, GoogleAuthRepository>();
        }

        private void ConfigureDatabase()
        {
            Services.AddDbContext<DataContext>(
                    options =>
                    {
                        options.UseSqlServer(Builder.Configuration.GetConnectionString("DefaultConnection"));
                    }
                );
        }

        private void ConfigureSeeders()
        {
            Services.AddTransient<Seed>();
        }
        private void ConfigureValidators()
        {
            // Validator Config
            Services.AddFluentValidationAutoValidation();
            Services.AddValidatorsFromAssemblyContaining<Program>();
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
        }

        private void ConfigureAutoMapper()
        {
            Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        private void ConfigureEmailConfiguration()
        {
            Services.Configure<MailSettings>(Builder.Configuration.GetSection("MailSettings"));
            Services.AddTransient<IEmailRepository, EmailRepository>();
        }

        public void Configure(WebApplicationBuilder app)
        {
            Services = app.Services;
            Builder = app;

            //app.Configuration.AddUserSecrets<Program>();

            ConfigureSeeders();
            ConfigureDatabase();
            ConfigureValidators();
            ConfigureAuthentication();
            ConfigureAutoMapper();
            ConfigureEmailConfiguration();
            ConfigureRepositories();
        }
    }
}
