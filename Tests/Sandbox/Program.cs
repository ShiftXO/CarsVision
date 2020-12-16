namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using CarsVision.Data;
    using CarsVision.Data.Common;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Data.Seeding;
    using CarsVision.Services;
    using CarsVision.Services.Data;
    using CarsVision.Services.Messaging;

    using CommandLine;
    using CsvHelper;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            // Seed data on application startup
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;

                return Parser.Default.ParseArguments<SandboxOptions>(args).MapResult(
                    opts => SandboxCode(opts, serviceProvider).GetAwaiter().GetResult(),
                    _ => 255);
            }
        }

        private static async Task<int> SandboxCode(SandboxOptions options, IServiceProvider serviceProvider)
        {
            var settingsService = serviceProvider.GetService<ISettingsService>();
            Console.WriteLine($"Count of settings: {settingsService.GetCount()}");

            // var carsScrapperService = serviceProvider.GetService<ICarsScrapperService>();

            // var properties = carsScrapperService.PopulateDb(3500);

            // string path = $"C:\\Users\\Vlad\\Desktop\\cars.bg-raw-data-{DateTime.Now:yyyy-MM-dd}.csv";
            // if (!File.Exists(path))
            // {
            //     // Create a file to write to.
            //     using (StreamWriter sw = File.CreateText(path))
            //     {
            //         sw.WriteLine("Hello");
            //         sw.WriteLine("And");
            //         sw.WriteLine("Welcome");
            //     }
            // }

            // using var csvWriter = new CsvWriter(new StreamWriter(File.OpenWrite($"C:\\Users\\Vlad\\Desktop\\project stuff\\cars.bg-raw-data-{DateTime.Now:yyyy-MM-dd}.csv"), Encoding.UTF8), CultureInfo.CurrentCulture);
            // csvWriter.WriteRecords(properties);

            // File.WriteAllText(
            //  $"cars.json",
            //  JsonConvert.SerializeObject(properties));
            return await Task.FromResult(0);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                    .UseLoggerFactory(new LoggerFactory()));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender, NullMessageSender>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<ICarsScrapperService, CarsScrapperService>();
        }
    }
}
