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
    public class EmailController : ApiController
    {

        public EmailController()
        {
           _email = new  testJsonDynamic.SendGrid.Emails();
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        public async Task<dynamic> Post()
        {
            try
            {
                //Usuario actual
                var item = RequestContext.Principal.Identity;
                string value = await Request.Content.ReadAsStringAsync();
                var entidad = System.Web.Helpers.Json.Decode(value);
               
                string recipientsString = Convert.ToString(entidad.recipients);
                var recipients = recipientsString.Trim().Replace(" ","").Split(',').ToList();

                return  _email.enviarCorreo(entidad.from, recipients, entidad.subject, entidad.mensajetext, entidad.mensajehtml);
            }
            catch (Exception ex)
            {
                return ex;
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

        internal SendGrid.Emails _email { get; set; }
    }
}