using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using AccidentalFish.AspNet.Identity.Azure;
using testJsonDynamic.Providers;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;


namespace testJsonDynamic
{
    public partial class Startup
    {   
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static Func<UserManager<TableUser>> UserManagerFactory { get; set; }
        public static string PublicClientId { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }


        static Startup()
        {           
            PublicClientId = "self";
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoftautentication;AccountKey=G943HAZgCsBqPwANE8tfKWaPq0FDM68gaUH4fCQz+W1NTGOBswZvQka1SFnquoYv+xrcPQQew7LQFcelJHycxw==";
            UserManagerFactory = () => new UserManager<TableUser>(new TableUserStore<TableUser>(connectionString));

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                //Para autenticacion con terceros se comentarea
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);           

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthOptions);           

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = new ApplicationOAuthBearerAuthenticationProvider(),
            });

            //Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "505952414500-pv2455cqm2g1kiqc6c5okctpidefjin1.apps.googleusercontent.com",
                ClientSecret = "2tsRLEoORWfb-iudS6KAuLEp",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "426977727468725",
                AppSecret = "9f8e5ab3ee84e576cf08a34df1fc4c80",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(facebookAuthOptions);
        }
    }
}
