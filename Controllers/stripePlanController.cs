using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    [Authorize]
    public class stripePlanController : ApiController
    {
        // GET: api/stripePlan
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/stripePlan/5
        public string Get(int id)
        {
            return "value";
        }

        public async Task<dynamic> Post()
        {
            try
            {
                var item = RequestContext.Principal.Identity;
                string value = await Request.Content.ReadAsStringAsync();
                var entidad = System.Web.Helpers.Json.Decode(value);
                
                var suscripcion = entidad.suscripcion;
                var custumer = entidad.costumer;
                var subscriptionService = new StripeSubscriptionService("sk_test_CBdkobSnlUEOyOjsLQ8fpqof");
                var result = subscriptionService.Create(custumer, suscripcion);
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        // PUT: api/stripePlan/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/stripePlan/5
        public void Delete(int id)
        {
        }
    }
}
