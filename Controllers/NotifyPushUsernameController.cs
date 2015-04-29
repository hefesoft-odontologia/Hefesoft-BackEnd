using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using testJsonDynamic.Bus;

namespace testJsonDynamic.Controllers
{
    public class NotifyPushUsernameController : ApiController
    {
        public NotifyPushUsernameController()
        {
            hub = Notifications.Instance.Hub;
        }


        // GET: api/NotifyPushUsername
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/NotifyPushUsername/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/NotifyPushUsername
        public async Task<HttpResponseMessage> Post()
        {
            string value = await Request.Content.ReadAsStringAsync();
            var entidad = System.Web.Helpers.Json.Decode(value);
            string pns = "";
            var message = entidad.mensaje;
            var to_tag = entidad.to_tag;

            var user = HttpContext.Current.User.Identity.Name;
            string[] userTag = new string[2];
            userTag[0] = "username:" + to_tag;
            userTag[1] = "from:" + user;

            var username = await hub.GetRegistrationsByTagAsync(userTag[0], 10);
            HttpStatusCode ret = HttpStatusCode.Accepted;

            foreach (RegistrationDescription item in username)
            {
                pns = setPns(pns, item);
                ret = await enviarPush(pns, message, user, userTag);
            }

            return Request.CreateResponse(ret);
        }

        private static string setPns(string pns, RegistrationDescription item)
        {
            if (item is WindowsRegistrationDescription)
            {
                pns = "wns";
            }
            else if (item is GcmRegistrationDescription)
            {
                pns = "gcm";
            }
            else if (item is AppleRegistrationDescription)
            {
                pns = "apns";
            }
            return pns;
        }

        private static async Task<HttpStatusCode> enviarPush(string pns, dynamic message, string user, string[] userTag)
        {
            Microsoft.ServiceBus.Notifications.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;


            switch (pns.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                "From " + user + ": " + message + "</text></binding></visual></toast>";
                    outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "gcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + message + "\"}}";
                    outcome = await Notifications.Instance.Hub.SendGcmNativeNotificationAsync(notif, userTag);
                    break;
            }

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.ServiceBus.Notifications.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }
            return ret;
        }

        // PUT: api/NotifyPushUsername/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/NotifyPushUsername/5
        public void Delete(int id)
        {
        }



        public Microsoft.ServiceBus.Notifications.NotificationHubClient hub { get; set; }
    }
}
