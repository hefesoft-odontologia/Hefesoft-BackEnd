using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.OAuth;

namespace testJsonDynamic.Providers
{
    public class ApplicationOAuthBearerAuthenticationProvider 
        : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            // try to find bearer token in a cookie 
            // (by default OAuthBearerAuthenticationHandler 
            // only checks Authorization header)
            var tokenCookie = context.OwinContext.Request.Cookies["BearerToken"];

            string bearerToken = context.Request.Query.Get("bearer_token");

            if (bearerToken != null)
            {
                string[] authorization = new string[] { "bearer " + bearerToken };
                context.Request.Headers.Add("Authorization", authorization);
            }

            //Aca se saca el token de autenticacion y se pone en el contexto
            if (context.Request.Headers.Any(a => a.Key == "Authorization") && context.Request.Headers.Get("Authorization").Any())
            {
                tokenCookie = context.Request.Headers.Get("Authorization");
            }

            if (!string.IsNullOrEmpty(tokenCookie))
                context.Token = tokenCookie;
            return Task.FromResult<object>(null);
        }

    }
}