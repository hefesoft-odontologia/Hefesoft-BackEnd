using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace testJsonDynamic.storage
{
    public partial class azureStorage
    {
        internal void insertEntityGroup(dynamic entidad)
        {
            var client = storageAccount.CreateCloudTableClient();

            client.DefaultRequestOptions = new TableRequestOptions()
            {
                PayloadFormat = TablePayloadFormat.JsonNoMetadata
            };


            var nombreTabla = entidad[0].nombreTabla;
            var table = client.GetTableReference(nombreTabla);
            table.CreateIfNotExists();
                
            //Se parte en trozos de 100 xq azure no soporta mas de 100 ingresos en un mismo batch
            var trozos = splitInChunks(entidad, 98);

            foreach (var listado100 in trozos)
            {
                var batchOperations = new TableBatchOperation();

                foreach (var item in listado100)
                {
                    proccessEntity(item, batchOperations);
                }

                table.ExecuteBatch(batchOperations);
            }
        }


        private dynamic proccessEntity(dynamic entidad, TableBatchOperation batchOperations)
        {
            try
            {               
                //Si la entidad esta en estado eliminar
                if (Convert.ToInt32(entidad.Estado_Entidad) == 2)
                {
                    string partitionKey = Convert.ToString(entidad.PartitionKey);
                    string rowkey = Convert.ToString(entidad.RowKey);

                    delete(entidad.nombreTabla, partitionKey, rowkey);
                }
                else
                {
                    if (Convert.ToBoolean(entidad.generarIdentificador))
                    {
                        //Generando ids consecutivos
                        entidad.RowKey = Variables_Globales.GetUniqueId().ToString();
                        entidad.generarIdentificador = false;
                    }

                    string partitionKey = Convert.ToString(entidad.PartitionKey);
                    string rowkey = Convert.ToString(entidad.RowKey);

                    dynamic entity = new DynamicObjectTableEntity(partitionKey, rowkey);

                    ReflectPropertyValue(entidad, ref entity);

                    var entidadInsertar = TableOperation.InsertOrReplace(entity);
                    batchOperations.Add(entidadInsertar);
                }

                return entidad;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private dynamic splitInChunks(dynamic entidades,int trozos)
        {
            int i = 0;
            var list = new List<dynamic>();
            var lstOf100 = new List<List<dynamic>>();

            foreach (var item in entidades)
            {
                if (i < trozos)
                {
                    list.Add(item);
                }
                else
                {
                    i = 0;
                    list.Add(item);
                    lstOf100.Add(list);

                    //Inicia de nuevo
                    List<dynamic> lst = new List<dynamic>();
                }

                i = i + 1;
            }

            //Cuando salga agregue los que queden faltando
            lstOf100.Add(list);
            return lstOf100;
        }

        internal dynamic getByRowKey(string nombreTabla, string rowkey)
        {
            try
            {
                var client = storageAccount.CreateCloudTableClient();

                client.DefaultRequestOptions = new TableRequestOptions()
                {
                    PayloadFormat = TablePayloadFormat.JsonNoMetadata
                };

                var table = client.GetTableReference(nombreTabla);

                TableQuery query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowkey));
                var result = table.ExecuteQuery(query);
                dynamic resultado = result.ToList();

                List<object> lst = new List<object>();

                foreach (var item in resultado)
                {
                    var elemento = new Expando();
                    elemento["PartitionKey"] = item.PartitionKey;
                    elemento["RowKey"] = item.RowKey;

                    foreach (var itemB in item.Properties)
                    {
                        var propiedad = ConvertToEntityProperty(itemB.Key, itemB.Value);
                        elemento[itemB.Key] = propiedad;
                    }

                    lst.Add(elemento.Properties);
                }
                
                return lst;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
