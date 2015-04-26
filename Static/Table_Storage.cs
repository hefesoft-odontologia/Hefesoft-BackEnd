using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.Estaticas
{
    public class Table_Storage
    {
        private static Azure.Table_Storage storage { get; set; }

        public static Azure.Table_Storage obtenerTableStorage()
        {
            if (storage == null)
            {
                storage = new Azure.Table_Storage();
            }

            return storage;
        }

    }
}
