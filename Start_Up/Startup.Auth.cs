﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using AccidentalFish.AspNet.Identity.Azure;
using testJsonDynamic.Providers;


namespace testJsonDynamic
{
    public partial class Startup
    {
        static Startup()
        {
           
            PublicClientId = "self";
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoftautentication;AccountKey=G943HAZgCsBqPwANE8tfKWaPq0FDM68gaUH4fCQz+W1NTGOBswZvQka1SFnquoYv+xrcPQQew7LQFcelJHycxw==";
            UserManagerFactory = () => new UserManager<TableUser>(new TableUserStore<TableUser>(connectionString));

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static Func<UserManager<TableUser>> UserManagerFactory { get; set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enable the application to use bearer tokens to authenticate users
            // app.UseOAuthBearerTokens(OAuthOptions);

            app.UseOAuthAuthorizationServer(OAuthOptions);
            
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new ApplicationOAuthBearerAuthenticationProvider(),
            });
        }
    }
}
