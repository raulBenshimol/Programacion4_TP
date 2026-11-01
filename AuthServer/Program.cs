using AuthServer.Services;
namespace AuthServer
{

    
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Singleton: la clave privada se carga una sola vez.
            builder.Services.AddSingleton<TokenService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

    // paquetes NuGet instalamos Microsoft.IdentityModel.JsonWebTokens. Es la librería que fabrica el JWT.



}
