using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.Start_Up
{
    public class OwinMiddleWareQueryStringExtractor : OwinMiddleware
    {

        public OwinMiddleWareQueryStringExtractor(OwinMiddleware next)
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            Console.WriteLine("Begin Request");
            Debug.WriteLine("------------" + context.Request.QueryString);
            Debug.WriteLine("-----------" + context.Request.Path);
            if (context.Request.Path.Value.Contains("/signalr"))
            {
                string bearerToken = context.Request.Query.Get("bearer_token");
                
                if (bearerToken != null)
                {
                    string[] authorization = new string[] { "bearer " + bearerToken };
                    context.Request.Headers.Add("Authorization", authorization);
                }
            }

            await Next.Invoke(context);
            Console.WriteLine("End Request");
        }
    }
}
