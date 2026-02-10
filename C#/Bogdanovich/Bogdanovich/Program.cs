using Bogdanovich.Infrastructure;

namespace Bogdanovich
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            IConfigurationBuilder configBuild = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configBuild.Build();
            AppConfig config = configuration.GetSection("Project").Get<AppConfig>()!;

            builder.Services.AddControllersWithViews();

            WebApplication app = builder.Build();



            app.UseStaticFiles();

            app.UseRouting();

            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();

        }
    }
}