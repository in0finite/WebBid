using Microsoft.Owin;
using Owin;

//[assembly: OwinStartupAttribute(typeof(WebApplication1.Startup))]

// assembly for signalR
[assembly: OwinStartup(typeof(WebApplication1.Startup))]

namespace WebApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           
            // for signalR
            app.MapSignalR();
        }
    }
}