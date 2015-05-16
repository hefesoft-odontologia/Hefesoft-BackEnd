using Hefesoft.Azure.Console.Entidades;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefesoft.Azure.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            listarTablas();

        }

        private static void listarTablas()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoft;AccountKey=dodn17DT7hBi3lXrWlvXihLS9J7xuItHLIpWLBZn2QEMdBHm02Lqxr055rNCpP5z3FhfcjjX3MhPy1Npk3VF3Q==";
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
            IEnumerable<CloudTable> tables = tableClient.ListTables();

            //validarTableExist(tableClient);

            CloudTable table = tableClient.GetTableReference("TmKeys");
            table.CreateIfNotExists();
            
            foreach (CloudTable item in tables)
            {   
                var policy = sharedKey();
                string sharedAccessSignature = item.GetSharedAccessSignature(policy);

                // Create a new customer entity.
                SharedKeyEntity shared = new SharedKeyEntity(item.Name, "1");
                shared.Key = sharedAccessSignature;
                shared.Expire = policy.SharedAccessExpiryTime.Value.ToString();

                if (policy.SharedAccessStartTime.HasValue)
                    shared.Start = policy.SharedAccessStartTime.Value.ToString();

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(shared);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }                
        }

        private static SharedAccessTablePolicy sharedKey()
        {
            SharedAccessTablePolicy policy = new SharedAccessTablePolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddYears(1),
                Permissions = SharedAccessTablePermissions.Query 
                             | SharedAccessTablePermissions.Add
                             | SharedAccessTablePermissions.Update
                             | SharedAccessTablePermissions.Delete
            };

            return policy;
        }

        private static bool validarTableExist(CloudTableClient tableClient)
        {
            var tableExist = tableClient.GetTableReference("Hola");
            var result = tableExist.Exists();
            CloudTable table = tableClient.GetTableReference("TmKeys");
            table.CreateIfNotExists();

            if (result == false)
            {
                tableExist.Create();                
                var policy = sharedKey();
                string sharedAccessSignature = tableExist.GetSharedAccessSignature(policy);

                // Create a new customer entity.
                SharedKeyEntity shared = new SharedKeyEntity(tableExist.Name, "1");
                shared.Key = sharedAccessSignature;
                shared.Expire = policy.SharedAccessExpiryTime.Value.ToString();

                if (policy.SharedAccessStartTime.HasValue)
                    shared.Start = policy.SharedAccessStartTime.Value.ToString();

                // Create the TableOperation that inserts the customer entity.
                TableOperation insertOperation = TableOperation.Insert(shared);

                // Execute the insert operation.
                table.Execute(insertOperation);
            }

            return result;
        }
    }
}
