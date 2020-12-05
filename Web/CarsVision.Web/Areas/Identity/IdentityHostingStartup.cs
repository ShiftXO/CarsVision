using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(CarsVision.Web.Areas.Identity.IdentityHostingStartup))]

namespace CarsVision.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}
