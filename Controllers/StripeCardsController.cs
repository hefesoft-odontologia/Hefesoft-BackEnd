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
    public class StripeCardsController : ApiController
    {
        // GET: api/StripeCards
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/StripeCards/5
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

                var cardService = new StripeCardService("sk_test_CBdkobSnlUEOyOjsLQ8fpqof");
                var result = cardService.Delete(entidad.customeId, entidad.subscriptionId);
                return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        // PUT: api/StripeCards/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/StripeCards/5
        public void Delete(int id)
        {
        }
    }
}
