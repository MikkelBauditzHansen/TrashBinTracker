using TrashBinTracker.Repo;

namespace TrashBinTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
            {
                     options.JsonSerializerOptions.Converters.Add(
                     new System.Text.Json.Serialization.JsonStringEnumConverter());
             });
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ITrashRepository, TrashRepositoryList>();
            builder.Services.AddSingleton<ILocationRepository, LocationRepositoryList>();

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