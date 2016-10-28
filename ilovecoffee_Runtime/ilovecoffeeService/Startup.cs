using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ilovecoffeeService.Startup))]

namespace ilovecoffeeService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}