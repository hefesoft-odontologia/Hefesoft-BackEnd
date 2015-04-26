using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Routing;
using System.Web.Security;
using testJsonDynamic.App_Start;
using testJsonDynamic.filters;
using testJsonDynamic.util.verbs;

namespace testJsonDynamic
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Toca comentarearlo cuando se integra con owin y katana
            //var cors = new EnableCorsAttribute("*", "*", "*");
            //config.EnableCors(cors);

            //config.MessageHandlers.Add(new MethodOverrideHandler());
            // Web API routes

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            var container = new UnityContainer();
            container.RegisterType<testJsonDynamic.storage.azureStorage>(new HierarchicalLifetimeManager());
            container.RegisterType<testJsonDynamic.storage.blobStorage>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.Filters.Add(new MyAuthenticationFilter());

        }
    }

}
