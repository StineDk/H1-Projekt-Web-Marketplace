using BlazorApp.Components;
using DomainModels;
using Microsoft.AspNetCore.Components;
using Service;


namespace BlazorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration Configuration = builder.Configuration;
            var connectionString = Configuration.GetConnectionString("DefaultConnection") ?? Environment.GetEnvironmentVariable("DefaultConnection");
            builder.Services.AddSingleton<DatabaseService>(sp => new DatabaseService(connectionString));
            builder.Services.AddSingleton<FiltreTypes>(sp => new FiltreTypes());
            

            builder.Services.AddScoped<AuthenticationService>();

            //to insert dummy data
            //builder.Services.AddSingleton<List<Item>>(sp => new DummyData().GenereteDummyItems());


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
