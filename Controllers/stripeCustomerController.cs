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
    public class stripeCustomerController : ApiController
    {
        // GET: api/stripeCustomer
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/stripeCustomer/5
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

                var myCustomer = new StripeCustomerCreateOptions();

                // set these properties if it makes you happy
                myCustomer.Email = entidad.email;
                myCustomer.Description = Convert.ToString(entidad.descripcion);

                // setting up the card
                myCustomer.Card = new StripeCreditCardOptions()
                {
                    // set this property if using a token
                    TokenId = entidad.token,

                };
                myCustomer.PlanId = Convert.ToString(entidad.planId);
                myCustomer.TaxPercent = 20;
                
                if(Convert.ToBoolean(entidad.tienecupon))
                {
                    myCustomer.CouponId = Convert.ToString(entidad.Cupon);
                }

                myCustomer.TrialEnd = DateTime.UtcNow.AddMonths(1);    
                myCustomer.Quantity = 1;

                var customerService = new StripeCustomerService("sk_test_CBdkobSnlUEOyOjsLQ8fpqof");
                StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                return stripeCustomer;
            }
            catch (Exception ex)
            {
                return ex;
            }

        }

        // PUT: api/stripeCustomer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/stripeCustomer/5
        public void Delete(int id)
        {
        }
    }
}
