using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hefesoft.Azure.Console.Entidades
{
    public class SharedKeyEntity : TableEntity
    {
        public SharedKeyEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public SharedKeyEntity() { }

        public string Key { get; set; }

        public string Expire { get; set; }

        public string Start { get; set; }
    }
}
