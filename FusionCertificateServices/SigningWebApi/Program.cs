using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.OpenApi.Models;

namespace SigningWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Signing API",
                    Version = "v1",
                    Description = "API for document signing and certificate management"
                });

                // Ignore Stream types in Swagger
                c.MapType<Stream>(() => new OpenApiSchema { Type = "string", Format = "binary" });
            });

            var app = builder.Build();
            var addresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses;
            if (addresses != null && addresses.Count > 0)
            {
                Console.WriteLine($"SigningWebApi listening on: {string.Join(", ", addresses)}");
            }
            else
            {
                // Fallback to configuration
                var urls = app.Configuration["urls"] ?? "http://localhost:5000;https://localhost:5001";
                Console.WriteLine($"SigningWebApi configured for: {urls}");
            }



            // Always enable Swagger for testing
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
