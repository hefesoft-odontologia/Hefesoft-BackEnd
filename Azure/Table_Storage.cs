using testJsonDynamic.Azure.MapUsersSample;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.Azure
{
    public class Table_Storage
    {
        public Table_Storage()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoft;AccountKey=dodn17DT7hBi3lXrWlvXihLS9J7xuItHLIpWLBZn2QEMdBHm02Lqxr055rNCpP5z3FhfcjjX3MhPy1Npk3VF3Q==";
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public ConnectionResult send(string who, string message, string to, dynamic Clients)
        {
            var result = new ConnectionResult();
            var table = GetConnectionTable();

            var query = new TableQuery<ConnectionEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                to));

            var queryResult = table.ExecuteQuery(query).ToList();
            if (queryResult.Count == 0)
            {
                result.tieneElementos = false;
                //Clients.Caller.showErrorMessage("The user is no longer connected.");
            }
            else
            {
                result.tieneElementos = true;
                result.Listado = queryResult.ToList();

                foreach (var entity in queryResult)
                {
                    Clients.Client(entity.RowKey).broadcastMessage(who, message);
                }
            }

            return result;
        }
        
        /// <summary>
        ///El objetivo es eliminar todos los registros de esta tabla que
        ///van quedandose vivos para eso se hace una validacion por fecha
        /// </summary>
        /// <param name="name"></param>
        internal void borrarAntiguos(string name)
        {   
            var result = new ConnectionResult();
            var table = GetConnectionTable();

            var query = new TableQuery<ConnectionEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                "PartitionKey",
                QueryComparisons.Equal,
                name));

            var queryResult = table.ExecuteQuery(query).ToList();
            if (queryResult.Any())
            {
                foreach (var entity in queryResult)
                {
                    var anioRow = entity.Timestamp.Year;
                    var mesRow = entity.Timestamp.Month;
                    var diaRow = entity.Timestamp.Day;


                    if (anioRow <= DateTime.Now.Year && mesRow <= DateTime.Now.Month && diaRow < DateTime.Now.Day)
                    {
                        delete(entity.PartitionKey, entity.RowKey);
                    }
                }
            }            
        }

        public void insert(dynamic name, dynamic Context)
        {
            var table = GetConnectionTable();
            table.CreateIfNotExists();

            var entity = new ConnectionEntity(
                name.ToLower(),
                Context.ConnectionId);
            var insertOperation = TableOperation.InsertOrReplace(entity);
            table.Execute(insertOperation);
        }

        public void delete(dynamic name, dynamic Context)
        {
            try
            {
                var table = GetConnectionTable();
                var deleteOperation = TableOperation.Delete(
                    new ConnectionEntity(name, Context.ConnectionId) { ETag = "*" });
                table.Execute(deleteOperation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void delete(string PartitionKey, string RowKey)
        {
            try
            {
                var table = GetConnectionTable();
                var deleteOperation = TableOperation.Delete(
                    new ConnectionEntity(PartitionKey, RowKey) { ETag = "*" });
                table.Execute(deleteOperation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        private CloudTable GetConnectionTable()
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoftautentication;AccountKey=G943HAZgCsBqPwANE8tfKWaPq0FDM68gaUH4fCQz+W1NTGOBswZvQka1SFnquoYv+xrcPQQew7LQFcelJHycxw==";
            storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("connection");
        }

        public CloudStorageAccount storageAccount { get; set; }        
    }
}
