using AccidentalFish.AspNet.Identity.Azure;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    public class PasswordController : ApiController
    {
        // GET: api/Password
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Password/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Password
        public async Task<dynamic> Post(JObject json)
        {
            try
            {
                var entidad = System.Web.Helpers.Json.Decode(json.ToString());
                using (UserManager<TableUser> userManager = testJsonDynamic.Startup.UserManagerFactory())
                {
                    return await userManager.ChangePasswordAsync(entidad.userId, entidad.currentPassword, entidad.newPassword);                
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // PUT: api/Password/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Password/5
        public void Delete(int id)
        {
        }
    }
}
