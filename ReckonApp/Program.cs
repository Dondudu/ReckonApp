using ReckonApp.Application.StringSearch.Mappings;
using ReckonApp.Domain.Services.StringMatcher;
using ReckonApp.Infrastructure;
using ReckonApp.Infrastructure.ExternalApi;
using ReckonApp.Models;

namespace ReckonApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Bind ApiSettings from appsettings.json
            builder.Services.Configure<ReckonApiSettings>(builder.Configuration.GetSection("ReckonApiSettings"));
            builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

            builder.Services.AddHttpClient<IReckonApiClient, ReckonApiClient>()
                .AddPolicyHandler(HttpRetryPolicy.GetRetryPolicy());

            builder.Services.AddSingleton(typeof(IStringMatchResultMapper), typeof(StringMatchResultMapper));
            builder.Services.AddSingleton(typeof(IStringSearchCommandResultMapper), typeof(StringSearchCommandResultMapper));
            builder.Services.AddScoped(typeof(IStringMatcher), typeof(BruteForceStringMatcher));

            // Register MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
