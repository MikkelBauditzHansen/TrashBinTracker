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

            string[] allowedCorsOrigins =
            {
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "https://shstarthtml-drfseveaedgbfeac.swedencentral-01.azurewebsites.net"
            };

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy =>
                    {
                        policy
                            .WithOrigins(allowedCorsOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            //builder.Services.AddControllers();

            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<TelegramService>();
            builder.Services.AddScoped<WeatherService>();
            builder.Services.AddHostedService<TelegramUpdateBackgroundService>();

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

            try
            {
                using IServiceScope scope = app.Services.CreateScope();

                TrashDbContext dbContext =
                    scope.ServiceProvider.GetRequiredService<TrashDbContext>();

                dbContext.Database.ExecuteSqlRaw(
                    """
                    IF COL_LENGTH('TrashBins', 'IsActiveSensorBin') IS NULL
                    BEGIN
                        ALTER TABLE TrashBins
                        ADD IsActiveSensorBin bit NOT NULL
                            CONSTRAINT DF_TrashBins_IsActiveSensorBin DEFAULT 0;
                    END;
                    """);
            }
            catch (Exception ex)
            {
                app.Logger.LogWarning(
                    ex,
                    "Could not ensure IsActiveSensorBin column exists.");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
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
