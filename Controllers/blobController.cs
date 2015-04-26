using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    [Authorize]
    public class blobController : ApiController
    {
        testJsonDynamic.storage.blobStorage azure;

        public blobController(testJsonDynamic.storage.blobStorage _azure)
        {
            azure = _azure;
        }

        public dynamic Get(string PartitionKey, string RowKey, string nombreTabla)
        {
            dynamic entidad = new ExpandoObject();
            entidad.PartitionKey = PartitionKey;
            entidad.RowKey = RowKey;
            entidad.nombreTabla = nombreTabla;
            return azure.get(entidad);
        }

        public dynamic Get(string PartitionKey, string nombreTabla)
        {
            try
            {
                return azure.getAll(PartitionKey, nombreTabla);
            }
            catch
            {
                return "Error";
            }
        }

        
        
        public async Task<dynamic> Post()
        {
            try
            {
                string value = await Request.Content.ReadAsStringAsync();
                var entidad = System.Web.Helpers.Json.Decode(value);
                var result = azure.insert(entidad);
                return result;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        
        public void Put(int id, [FromBody]string value)
        {
        }

        
        public void Delete(string PartitionKey, string RowKey, string nombreTabla)
        {
            azure.Delete(PartitionKey, RowKey, nombreTabla);
        }
    }
}
