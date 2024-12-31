using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using SalesTrackerMVC.Models;

namespace SalesTrackerMVC
{
    public class EbayToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DB Context
            builder.Services.AddDbContext<SalesDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDB") ?? throw new InvalidOperationException("Connection string 'LocalDB' not found.")));

            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                //pattern: "{controller=Sales}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
