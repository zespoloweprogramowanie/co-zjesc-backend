using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WhatToEat.Startup))]
namespace WhatToEat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
