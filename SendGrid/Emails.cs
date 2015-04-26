using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;

namespace testJsonDynamic.SendGrid
{
    class Emails
    {
        public Emails()
        {
            username = "azure_4853b6710d24d12475892a7b6610a306@azure.com";
            pswd = "p62gYkHCSg6ORRx";
            credentials = new NetworkCredential(username, pswd);
        }
        
        public async Task<bool> enviarCorreo(string from,  List<String> recipients, string subject ,string mensajeText, string mensajeHtml)
        {
            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress(from);

            myMessage.AddTo(recipients);

            myMessage.Subject = subject;

            //Add the HTML and Text bodies
            myMessage.Html = mensajeHtml;
            myMessage.Text = mensajeText;

            var transportWeb = new Web(credentials);

            // Send the email.
            // You can also use the **DeliverAsync** method, which returns an awaitable task.
            await transportWeb.DeliverAsync(myMessage);
            return true;
        }


        public string username { get; set; }

        public string pswd { get; set; }

        public NetworkCredential credentials { get; set; }
    }
}
