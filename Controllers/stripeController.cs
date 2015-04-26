using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace testJsonDynamic.Controllers
{
    public class stripeController : ApiController
    {
        testJsonDynamic.storage.azureStorage _azure;

        public stripeController(testJsonDynamic.storage.azureStorage azure)  
        {            
            _azure = azure;
        }

        // GET api/<controller>
        public async Task<HttpStatusCodeResult> Get()
        {
            string value = await Request.Content.ReadAsStringAsync();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        // GET api/<controller>/5
        public async Task<HttpStatusCodeResult> Get(string value)
        {
            string valor = await Request.Content.ReadAsStringAsync();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        

        // POST api/<controller>
        public async Task<HttpStatusCodeResult> Post()
        {
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                var obj = JObject.Parse(value);               
                _azure.stripe(obj);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

    }
}
