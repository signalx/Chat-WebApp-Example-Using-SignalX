using Microsoft.Owin;
using SignalXChat.Web.App_Start;

[assembly: OwinStartup(typeof(StartUp))]

namespace SignalXChat.Web.App_Start
{
    using Microsoft.AspNet.SignalR;
    using Owin;
    using SignalXLib.Lib;

    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDynamicFiles("/public");

            GlobalHost.Configuration.DefaultMessageBufferSize = 100;
            SignalX.Instance.AllowDynamicServer = true;
            app.Map(
                "/signalr",
                map =>
                {
                    //map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration();

                    map.RunSignalR(hubConfiguration);
                });
        }
    }
}