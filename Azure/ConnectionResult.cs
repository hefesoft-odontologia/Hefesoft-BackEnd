using testJsonDynamic.Azure.MapUsersSample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testJsonDynamic.Azure
{
    public class ConnectionResult
    {
        public ConnectionResult()
        {
            Listado = new List<ConnectionEntity>();
            tieneElementos = true;
        }

        public List<ConnectionEntity> Listado { get; set; }

        public bool tieneElementos { get; set; }
    }
}
