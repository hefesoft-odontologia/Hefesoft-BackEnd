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
    public class stripeSubscriptionController : ApiController
    {
        // GET: api/stripeSubscription
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/stripeSubscription/5
        public dynamic Get(string customer)
        {
            var hefesoftStripe = new StripSubscriptionHefesoft();
            var customerService = new StripeCustomerService("sk_test_CBdkobSnlUEOyOjsLQ8fpqof");
            StripeCustomer stripeCustomer = customerService.Get(customer);
            hefesoftStripe.StripeCustomer = stripeCustomer;

            foreach (var item in stripeCustomer.StripeSubscriptionList.StripeSubscriptions)
            {
                hefesoftStripe.FechaInicialString = item.PeriodStart.Value.ToShortDateString();
                hefesoftStripe.FechaFinalString = item.PeriodEnd.Value.ToShortDateString();
                
                hefesoftStripe.FechaInicial = item.PeriodStart.Value.ToBinary();
                hefesoftStripe.FechaFinal = item.PeriodEnd.Value.ToBinary();
                
                hefesoftStripe.FechaActual = DateTime.Now;
                hefesoftStripe.FechaActualString = DateTime.Now.ToShortDateString();
                hefesoftStripe.FechaActualLong = DateTime.Now.ToBinary();
            }

            return hefesoftStripe;
        }

        public async Task<dynamic> Post()
        {
            try
            {
                var item = RequestContext.Principal.Identity;
                string value = await Request.Content.ReadAsStringAsync();
                var entidad = System.Web.Helpers.Json.Decode(value);

                var subscriptionService = new StripeSubscriptionService("sk_test_CBdkobSnlUEOyOjsLQ8fpqof");
               var result = subscriptionService.Cancel(entidad.customerId, entidad.subscriptionId);
               return result;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }


        // PUT: api/stripeSubscription/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/stripeSubscription/5
        public void Delete(int id)
        {
        }
    }

    public class StripSubscriptionHefesoft
    {
        public StripeCustomer StripeCustomer { get; set; }
        public long FechaInicial { get; set; }
        public long FechaFinal { get; set; }

        public StripSubscriptionHefesoft()
        {
            StripeCustomer = new StripeCustomer();
        }

        public string FechaInicialString { get; set; }

        public string FechaFinalString { get; set; }

        public string FechaActualString { get; set; }
        public long FechaActualLong { get; set; }

        public DateTime FechaActual { get; set; }
    }
}
