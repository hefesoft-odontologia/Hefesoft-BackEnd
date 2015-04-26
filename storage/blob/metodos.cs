using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.storage
{
    public partial class blobStorage
    {
        private CloudTable table;
        private CloudBlobContainer container;
        public CloudStorageAccount storageAccount { get; set; }
        public azureStorage tableStorage { get; set; }

        public blobStorage()
        {
            tableStorage = new azureStorage();
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=hefesoft;AccountKey=dodn17DT7hBi3lXrWlvXihLS9J7xuItHLIpWLBZn2QEMdBHm02Lqxr055rNCpP5z3FhfcjjX3MhPy1Npk3VF3Q==";
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        public void inicializarContenedor(dynamic entidad)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();

            //El nombre de la tabla debe estar en minuscula
            this.container = blobClient.GetContainerReference(entidad.nombreTabla);
            this.container.CreateIfNotExists();
        }

        public void Delete(string partitionKey, string rowKey, string nombreTabla)
        {
            dynamic elemento = new ExpandoObject();
            elemento.nombreTabla = nombreTabla;
            inicializarContenedor(elemento);

            DeleteDocument(partitionKey, rowKey, nombreTabla);
        }

        //Se encarga de subir la entidad al blob
        internal Uri UploadDocument(string partitionKey, string rowKey, string document, string nombreTabla)
        {
            var filename = string.Format(@"{2}\{0}\{1}.json", partitionKey, rowKey, nombreTabla);
            var blockBlob = this.container.GetBlockBlobReference(filename);
            blockBlob.Metadata.Add("PartitionKey", partitionKey);
            blockBlob.Metadata.Add("RowKey", rowKey);

            using (var memory = new MemoryStream())
            using (var writer = new StreamWriter(memory))
            {
                writer.Write(document);
                writer.Flush();
                memory.Seek(0, SeekOrigin.Begin);

                blockBlob.UploadFromStream(memory);
            }

            blockBlob.Properties.ContentType = "application/json";
            blockBlob.SetProperties();
            return blockBlob.Uri;
        }

        internal string DownloadDocument(string blobName)
        {
            var blockBlob = this.container.GetBlockBlobReference(blobName);

            using (var memory = new MemoryStream())
            using (var reader = new StreamReader(memory))
            {
                blockBlob.DownloadToStream(memory);
                memory.Seek(0, SeekOrigin.Begin);

                return reader.ReadToEnd();
            }
        }


        //Metodo en construccionpara recuperar un blob por otras caracteristicas
        internal string DownloadDocument(dynamic entidad)
        {
            dynamic entidadResult = tableStorage.getASingle(entidad.nombreTabla, entidad.partionKey, entidad.rowKey);
            var filename = string.Format(@"{2}\{0}\{1}.json", entidad.partitionKey, entidad.rowKey, entidad.nombreTabla);

            var blockBlob = this.container.GetBlockBlobReference(filename);

            using (var memory = new MemoryStream())
            using (var reader = new StreamReader(memory))
            {
                blockBlob.DownloadToStream(memory);
                memory.Seek(0, SeekOrigin.Begin);

                return reader.ReadToEnd();
            }
        }

        internal void DeleteDocument(string partitionKey, string rowKey, string nombreTabla)
        {
            var blobName = string.Format(@"{2}\{0}\{1}.json", partitionKey, rowKey, nombreTabla);
            var blockBlob = this.container.GetBlockBlobReference(blobName);
            blockBlob.Delete(DeleteSnapshotsOption.IncludeSnapshots);
        }

    }
}
