using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace testJsonDynamic
{
    public class ChatHub : Hub
    {
        public void Send(string usuario, dynamic entity)
        {
            //No funciona con los hubs
            var name = Context.QueryString.Get("usuario");
            var datos = System.Web.Helpers.Json.Decode(entity);

            if (string.IsNullOrEmpty(usuario))
            {
                usuario = "no indicado";
            }

            //Se debe cambiar el primero por who
            //Y hay que crear un sistema de autenticacion para que el nombre llegue en el identity
            var result = testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().send(usuario, datos.mensaje, datos.to, Clients);            
        }


        public override Task OnConnected()
        {
            var name = Context.QueryString.Get("usuario");
            
            if(string.IsNullOrEmpty(name))
            {
                name = "no indicado";
            }

            testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().insert(name, this.Context);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = Context.User.Identity.Name;

            if (string.IsNullOrEmpty(name))
            {
                name = "no indicado";
            }

            testJsonDynamic.Estaticas.Table_Storage.obtenerTableStorage().delete(name, this.Context);
            return base.OnDisconnected(stopCalled);
        }
    }
}