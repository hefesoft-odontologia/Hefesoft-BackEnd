//using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    public class SignalRController : ApiController
    {
        // GET: api/SignalR
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SignalR/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SignalR
        public void Post([FromBody]string value)
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //context.Clients.
        }

        // PUT: api/SignalR/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SignalR/5
        public void Delete(int id)
        {
        }
    }
}
