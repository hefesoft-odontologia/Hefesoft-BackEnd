﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ServiceBus.Notifications;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Web;
using testJsonDynamic.Bus;

namespace testJsonDynamic.Controllers
{
    [Authorize]
    public class RegisterController   : ApiController
    {
        private NotificationHubClient hub;

        public RegisterController()
        {
            hub = Notifications.Instance.Hub;
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        // POST api/register
        // This creates a registration id
        public async Task<string> Post()
        {
            string value = await Request.Content.ReadAsStringAsync();
            var entidad = System.Web.Helpers.Json.Decode(value);
            string handle = entidad.key;


            string newRegistrationId = null;

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null) 
                newRegistrationId = await hub.CreateRegistrationIdAsync();

            return newRegistrationId;
        }

        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id        
        public async Task<HttpResponseMessage> Put()
        {
            string value = await Request.Content.ReadAsStringAsync();
            var entidad = System.Web.Helpers.Json.Decode(value);
            string id = entidad.idhubazure;
            string[] tag = Convert.ToString(entidad.tag).Split(',');

            DeviceRegistration deviceUpdate = new DeviceRegistration() 
            { 
                Platform = entidad.platform, 
                Handle = entidad.key,
                Tags = tag
            };

            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            //registration.RegistrationId = id;
            var username = HttpContext.Current.User.Identity.Name;
            registration = await hub.GetRegistrationAsync<RegistrationDescription>(entidad.idhubazure);
            registration.Tags = new HashSet<string>();
            registration.Tags.Add("username:" + username);
            

            // Los tags no pueden ir con espacios
            foreach (var item in deviceUpdate.Tags)
            {
                var tagString = item.Replace(" ", "").Trim();
                registration.Tags.Add(tagString);
            }
            
            

            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                ReturnGoneIfHubResponseIsGone(e);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/register/5
        public async Task<HttpResponseMessage> Delete(string id)
        {
            await hub.DeleteRegistrationAsync(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        {
            var webex = e.InnerException as WebException;
            if (webex.Status == WebExceptionStatus.ProtocolError)
            {
                var response = (HttpWebResponse)webex.Response;
                if (response.StatusCode == HttpStatusCode.Gone)
                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
            }
        }
     
    }
}