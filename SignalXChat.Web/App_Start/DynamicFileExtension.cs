namespace SignalXChat.Web.App_Start
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using Owin;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public static class DynamicFileExtension
    {
        public static void UseDynamicFiles(this IAppBuilder app, string baseDirectory)
        {
            app.Use(
                new Func<AppFunc, AppFunc>(
                    next => async env =>
                    {
                        var requestBody = (Stream)env["owin.RequestBody"];
                        var requestHeaders = (IDictionary<string, string[]>)env["owin.RequestHeaders"];
                        string requestMethod = (string)env["owin.RequestMethod"];
                        string requestPath = (string)env["owin.RequestPath"];
                        string requestPathBase = (string)env["owin.RequestPathBase"];
                        string requestProtocol = (string)env["owin.RequestProtocol"];
                        string requestQueryString = (string)env["owin.RequestQueryString"];
                        string requestScheme = (string)env["owin.RequestScheme"];
                        var responseBody = (Stream)env["owin.ResponseBody"];
                        var responseHeaders = (IDictionary<string, string[]>)env["owin.ResponseHeaders"];
                        string owinVersion = (string)env["owin.Version"];
                        var cancellationToken = (CancellationToken)env["owin.CallCancelled"];
                        string uri = (string)env["owin.RequestScheme"] + "://" + requestHeaders["Host"].First() +
                            (string)env["owin.RequestPathBase"] + (string)env["owin.RequestPath"];

                        if (env["owin.RequestQueryString"] != "")
                            uri += "?" + (string)env["owin.RequestQueryString"];

                        string res = string.Format("{0} {1}", requestMethod, uri);

                        if (requestPath == "/" || requestPath == "\\")
                            requestPath = "/index.html";

                        if (requestMethod == "GET" && requestScheme == "http")
                        {
                            string fullpath = HttpContext.Current.Request.MapPath("~" + baseDirectory + requestPath);
                            if (File.Exists(fullpath))
                            {
                                using (FileStream file = File.OpenRead(fullpath))
                                {
                                    await file.CopyToAsync(responseBody);
                                }

                                //var mime = MimeMapping.GetMimeMapping(fullpath);
                                //responseHeaders.Add("Content-Type", new[] { mime });
                                return;
                            }
                        }

                        await next.Invoke(env);
                    }));
        }
    }
}