using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AccountingSystem.Infrastructure.Data;

namespace AccountingSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    "Server=localhost\\SQLEXPRESS;Database=AccountingSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
                ));

            services.AddScoped<UI.MenuConsole>();

            var serviceProvider = services.BuildServiceProvider();

            var menu = serviceProvider.GetRequiredService<UI.MenuConsole>();
            menu.MainMenu();
        }
    }
}