using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TrashBinTracker.Data;
using TrashBinTracker.Repo;
using TrashBinTracker.Service;


namespace TrashBinTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAll",
                                          policy =>
                                          {
                                              policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                                          });
            });
            builder.Services.AddControllers();
            builder.Services.AddHttpClient<TelegramService>();

            builder.Services.AddHttpClient<TelegramUpdateBackgroundService>();

            builder.Services.AddHostedService<TelegramUpdateBackgroundService>();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<TelegramService>();
            builder.Services.AddDbContext<TrashDbContext>(options =>
                        options.UseSqlServer(
                        builder.Configuration.GetConnectionString("TrashDb")));
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
            {
                     options.JsonSerializerOptions.Converters.Add(
                     new System.Text.Json.Serialization.JsonStringEnumConverter());
             });
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // Register repositories for DI. NotificationRepo now depends on ITrashRepository,
            // so let the container construct it.
            builder.Services.AddScoped<ITrashRepository, TrashRepositoryDB>();
            builder.Services.AddScoped<ILocationRepository, LocationRepositoryDB>();
            builder.Services.AddScoped<INotificationRepo, NotificationRepositoryDB>();
            builder.Services.AddScoped<IEmptyHistoryRepo, EmptyHistoryRepoDB>();

            // ? TILFØJ CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy.WithOrigins("http://127.0.0.1:5500")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });



            //JWT Authentication

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddAuthorization();




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI();


            // ? Aktiver JWT-AUTHENTICATION
            app.UseAuthentication(); // Checks "Who are you?"
            app.UseAuthorization();  // Checks "Are you allowed to be here?"



            app.MapControllers();

            app.Run();
        }
    }
}