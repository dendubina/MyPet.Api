using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyPet.Api.Middlewares;
using MyPet.Api.Models.EmailModels;
using MyPet.Api.SignalRHub;
using MyPet.BLL.Interfaces;
using MyPet.BLL.Models.EmailModels;
using MyPet.BLL.Services;
using MyPet.DAL.EF;
using MyPet.DAL.Interfaces;
using MyPet.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPet.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddScoped<IAdvertisementService, AdvertisementService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IChatService, ChatService>();

            services.Configure<EmailConfig>(Configuration.GetSection("EmailConfiguration"));            

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllers();

            services.AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyPet.Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });            

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["JwtIssuer"],
                    ValidAudience = Configuration["JwtIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
                    ValidateIssuerSigningKey = true,

                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                };

                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];                       
                        var path = context.HttpContext.Request.Path.ToString();

                        // если запрос направлен хабу
                        if (!string.IsNullOrWhiteSpace(accessToken) && (path.Contains("hubs/chat")))
                        {
                            // получаем токен из строки запроса
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                options.User.RequireUniqueEmail = true;
            });

            /*services.AddCors(options => options.AddDefaultPolicy(config => config
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            //.AllowCredentials()
            ));*/
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(p =>
                {
                    p.AllowAnyHeader();
                    p.WithOrigins("http://localhost:3000", "http://localhost:3001");
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                    p.AllowCredentials();
                    p.Build();
                });
            });

            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;               
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyPet.Api v1"));
            }

            app.UseCors();

            app.UseRouting();                        

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Add("Cache-Control", "public, max-age=3600");
                }
            });

            app.UseAuthentication();
            app.UseAuthorization();            

            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("hubs/chat");
            });
        }
    }
}
