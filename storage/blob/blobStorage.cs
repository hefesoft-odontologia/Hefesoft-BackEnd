using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace testJsonDynamic.storage
{
    public partial class blobStorage
    {   
        public dynamic get(dynamic entidad)
        {
            inicializarContenedor(entidad);

            var blobName = string.Format(@"{2}\{0}\{1}.json", entidad.PartitionKey, entidad.RowKey, entidad.nombreTabla);
            var document = this.DownloadDocument(blobName);
            return System.Web.Helpers.Json.Decode(document);
        }

        public IEnumerable<dynamic> getAll(string partitionKey, string nombreTabla)
        {
            dynamic elemento = new ExpandoObject();
            elemento.nombreTabla = nombreTabla;
            inicializarContenedor(elemento);
            
            var listItems = this.container
                .GetDirectoryReference(nombreTabla + "/" + partitionKey).ListBlobs();

            return listItems.OfType<CloudBlockBlob>()
                .Select(x => this.DownloadDocument(x.Name))
                .Select(document => System.Web.Helpers.Json.Decode(document));
        }

        //Get container por terminos busqueda
        public IEnumerable<dynamic> getAll(string partitionKey, string nombreTabla, string terminosBusqueda)
        {
            dynamic elemento = new ExpandoObject();
            elemento.nombreTabla = nombreTabla;
            inicializarContenedor(elemento);

            var listItems = this.container
                .GetDirectoryReference(nombreTabla + "/" + partitionKey)
                .ListBlobs();

            var result = listItems.OfType<CloudBlockBlob>()
                .Where(b => b.Name.Contains(terminosBusqueda))
                .Select(x => this.DownloadDocument(x.Name))
                .Select(document => System.Web.Helpers.Json.Decode(document));

            return result;
        }

        public IEnumerable<dynamic> getPaginated(string partitionKey, string nombreTabla, string terminosBusqueda, int take, int skip)
        {
            dynamic elemento = new ExpandoObject();
            elemento.nombreTabla = nombreTabla;
            inicializarContenedor(elemento);

            var listItems = this.container
                .GetDirectoryReference(nombreTabla + "/" + partitionKey)
                .ListBlobs();

            var result = listItems.OfType<CloudBlockBlob>()
                .Where(b => b.Name.Contains(terminosBusqueda))
                .Take(take)
                .Skip(skip)
                .Select(x => this.DownloadDocument(x.Name))
                .Select(document => System.Web.Helpers.Json.Decode(document));

            return result;
        }
        

        /// <summary>
        /// El nomde del blob debe estar en minuscula
        /// </summary>
        /// <param name="entidad"></param>
        public object insert(dynamic entidad)
        {
            inicializarContenedor(entidad);

            //Si la entidad esta en estado eliminar
            if (Convert.ToInt32(entidad.Estado_Entidad) == 2)
            {
                string partitionKey = Convert.ToString(entidad.PartitionKey);
                string rowkey = Convert.ToString(entidad.RowKey);

                Delete(entidad.PartitionKey, entidad.RowKey, entidad.nombreTabla);
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


                var document = JsonConvert.SerializeObject(entidad, Newtonsoft.Json.Formatting.Indented);
                var blobUrl = (Uri)UploadDocument(partitionKey, rowkey, document, entidad.nombreTabla);
                saveToTableStorage(entidad, blobUrl);
            }
            return entidad;
        }

        internal void saveToTableStorage(dynamic entidad, Uri blobUrl, tipo_Reflect reflectProperties =  tipo_Reflect.reflect_Json)
        {
            try
            {
                string partitionKey = Convert.ToString(entidad.PartitionKey);
                string rowkey = Convert.ToString(entidad.RowKey);

                //Tener referencia a donde se guarda el blob 
                //Para poder recuperarlo por caracteristicasdiferentes al nombre
                dynamic dynamicObject = new DynamicObjectTableEntity(entidad.PartitionKey, entidad.RowKey);
                dynamicObject.url = blobUrl.AbsolutePath;
                dynamicObject.blobName = string.Format(@"{2}\{0}\{1}.json", partitionKey, rowkey, entidad.nombreTabla);

                if (reflectProperties == tipo_Reflect.reflect_Json)
                {
                    ReflectPropertyValue(entidad, ref dynamicObject);
                }
                else if (reflectProperties == tipo_Reflect.reflect_Dynamic)
                {
                    reflectDynamic(entidad, dynamicObject);
                }

                tableStorage.insert(dynamicObject, entidad.nombreTabla);
            }
            catch
            {

            }
        }

        private void reflectDynamic(dynamic entidad, dynamic dynamicObject)
        {
            var entidadIterar = (IDictionary<string, object>)((ExpandoObject)entidad);

            foreach (var item in entidadIterar)
            {
                //Console.WriteLine("Name: {0}, Value: {1}",prop.Name, prop.GetValue(myVar,null));
                var nombre = item.Key;
                var valor = item.Value;
                var PropiedadEntidad = ConvertToEntityProperty(nombre, valor);

                if (!(nombre == "PartitionKey" || nombre == "RowKey"))
                {
                    dynamicObject.properties.Add(nombre, PropiedadEntidad);
                }
            }
        }

      

        public void ReflectPropertyValue(dynamic source, ref dynamic entidadGuardar)
        {

            var propiedadesElemento = source as DynamicJsonObject;
            var dynamicTableEntity = new DynamicObjectTableEntity();

            if (propiedadesElemento != null)
            {
                foreach (var item in propiedadesElemento.GetDynamicMemberNames())
                {
                    if (!(item == "PartitionKey" || item == "RowKey"))
                    {
                        try
                        {
                            var valor = ConvertToEntityProperty(item, (object)source[item]);

                            if (valor != null)
                            {
                                if (!((DynamicObjectTableEntity)entidadGuardar).properties.Any(a => a.Key == item))
                                {
                                    ((DynamicObjectTableEntity)entidadGuardar).properties.Add(item, valor);

                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }

        }

        public EntityProperty ConvertToEntityProperty(string key, object value)
        {
            if (value == null) return new EntityProperty((string)null);
            if (value.GetType() == typeof(byte[]))
                return new EntityProperty((byte[])value);
            if (value.GetType() == typeof(bool))
                return new EntityProperty((bool)value);
            if (value.GetType() == typeof(DateTimeOffset))
                return new EntityProperty((DateTimeOffset)value);
            if (value.GetType() == typeof(DateTime))
                return new EntityProperty((DateTime)value);
            if (value.GetType() == typeof(double))
                return new EntityProperty((double)value);
            if (value.GetType() == typeof(Guid))
                return new EntityProperty((Guid)value);
            if (value.GetType() == typeof(int))
                return new EntityProperty((int)value);
            if (value.GetType() == typeof(long))
                return new EntityProperty((long)value);
            if (value.GetType() == typeof(string))
                return new EntityProperty((string)value);
            else
            {
                return null;
            }
        }

        internal dynamic insertImagen(dynamic entidad, byte[] Imagen)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            //El nombre de la tabla debe estar en minuscula
            this.container = blobClient.GetContainerReference(entidad.folder);
            this.container.CreateIfNotExists();

            this.container.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            var blockBlob = this.container.GetBlockBlobReference(entidad.name);

            blockBlob.UploadFromByteArray(Imagen, 0, Imagen.Length);
            blockBlob.Properties.ContentType = "image/jpg";
            blockBlob.SetProperties();
            return blockBlob.Uri;

        }
    }

   

    public enum tipo_Reflect
    {
        reflect_Json = 1,
        reflect_Dynamic = 2,
    }
}
