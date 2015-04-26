using AccidentalFish.AspNet.Identity.Azure;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    public class SignUpController : ApiController
    {
        public SignUpController()
        {

        }

        // GET: api/SignUp
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SignUp/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SignUp
        public async Task<dynamic> Post(JObject json)
        {
            try
            {
                var entidad = System.Web.Helpers.Json.Decode(json.ToString());
                using (UserManager<TableUser> userManager = testJsonDynamic.Startup.UserManagerFactory())
                {
                    return await userManager.CreateAsync(new TableUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = entidad.email,
                        UserName = entidad.username,
                    }, entidad.password
                );
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // PUT: api/SignUp/5
        public async Task<dynamic> Put(JObject json)
        {
            try
            {
                var entidad = System.Web.Helpers.Json.Decode(json.ToString());
                using (UserManager<TableUser> userManager = testJsonDynamic.Startup.UserManagerFactory())
                {
                    return await userManager.UpdateAsync(new TableUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = entidad.email,
                        UserName = entidad.username,
                    }
                );
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // DELETE: api/SignUp/5
        public void Delete(int id)
        {
        }
    }
}
