using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OpenGate.Startup))]
namespace OpenGate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
