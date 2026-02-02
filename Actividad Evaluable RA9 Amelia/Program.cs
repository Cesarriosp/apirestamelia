using Actividad_Evaluable_RA9_Amelia.Services;

namespace Actividad_Evaluable_RA9_Amelia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar servicios para MVC
            builder.Services.AddControllersWithViews();

            // Configurar HttpClient para el servicio de API
            builder.Services.AddHttpClient<IndicadoresApiService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                // Aceptar certificados SSL en desarrollo
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    // Para desarrollo, permitir certificados SSL autofirmados
                    ServerCertificateCustomValidationCallback = 
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            });

            // Registrar el servicio de API
            builder.Services.AddScoped<IndicadoresApiService>();

            var app = builder.Build();

            // Configurar el pipeline de la aplicación
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Configurar las rutas MVC
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Indicadores}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
