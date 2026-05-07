using TrashBinTracker.Repo;
using Microsoft.EntityFrameworkCore;
using TrashBinTracker.Data;

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

            // ? AKTIVÉR CORS (skal være før Authorization!)
            app.UseCors("AllowFrontend");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}