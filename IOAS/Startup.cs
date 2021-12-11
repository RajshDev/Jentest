using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IOAS.Startup))]
namespace IOAS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
