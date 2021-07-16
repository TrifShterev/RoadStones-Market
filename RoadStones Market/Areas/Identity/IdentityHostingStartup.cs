using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(RoadStones_Market.Areas.Identity.IdentityHostingStartup))]
namespace RoadStones_Market.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}