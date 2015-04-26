using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace testJsonDynamic.Controllers
{
    public class blobBusquedasController : ApiController
    {
        testJsonDynamic.storage.blobStorage azure = new storage.blobStorage();

        public dynamic Get(string PartitionKey, string nombreTabla, string terminosBusqueda)
        {
            return azure.getAll(PartitionKey, nombreTabla, terminosBusqueda);
        }

        public dynamic Get(string PartitionKey, string nombreTabla, string terminosBusqueda, int take, int skip)
        {
            return azure.getPaginated(PartitionKey, nombreTabla, terminosBusqueda, take, skip);
        }
    }
}
