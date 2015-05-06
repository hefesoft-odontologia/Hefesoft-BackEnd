using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.Azure
{
    namespace MapUsersSample
    {
        public class ConnectionEntity : TableEntity
        {
            public ConnectionEntity() { }

            public ConnectionEntity(string userName, string connectionID)
            {                
                this.PartitionKey = userName;
                this.RowKey = connectionID;
            }            
        }
    }
}
