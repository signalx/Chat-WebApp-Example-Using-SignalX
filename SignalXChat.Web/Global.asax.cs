﻿namespace SignalXChat.Web
{
    using System.Web;
    using System.Web.Http;
    using SignalXChat.Web.Reactive;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            SignalXHubs.Register();
        }
    }
}