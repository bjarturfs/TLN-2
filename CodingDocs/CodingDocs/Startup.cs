using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodingDocs.Startup))]
namespace CodingDocs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
