using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_5204_LearnCodeApp.Startup))]
namespace _5204_LearnCodeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
