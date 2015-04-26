using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using testJsonDynamic.Start_Up;
using Microsoft.AspNet.SignalR;
[assembly: OwinStartup(typeof(testJsonDynamic.Startup))]

namespace testJsonDynamic
{
    public partial  class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR(new HubConfiguration() { EnableJSONP = true });

            //se usa para interceptar los llamados
            //app.Use(typeof(OwinMiddleWareQueryStringExtractor));
        }
    }
}
