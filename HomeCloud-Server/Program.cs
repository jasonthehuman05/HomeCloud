using HomeCloud_Server.Auth;
using HomeCloud_Server.Models;
using HomeCloud_Server.Services;

namespace HomeCloud_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
            builder.Services.AddSingleton<DatabaseService>();

            builder.Services.Configure<ConfigurationService>(builder.Configuration.GetSection("ConfigurationSettings"));

            //builder.Services.AddAuthentication("AuthScheme");

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



            app.UseMiddleware<ApiKeyAuthMiddleware>();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}