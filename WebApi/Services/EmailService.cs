using Microsoft.AspNet.Identity;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace WebApi.Services {
  public class EmailService : IIdentityMessageService {
    public async Task SendAsync(IdentityMessage message) {
      await configSendGridasync(message);
    }

    private async Task configSendGridasync(IdentityMessage message) {
      var myMessage = new SendGridMessage();
      myMessage.AddTo(message.Destination);
      myMessage.From = new System.Net.Mail.MailAddress("test@test.com", "Test Mail");
      myMessage.Subject = message.Subject;
      myMessage.Text = message.Body;
      myMessage.Html = message.Body;

      var credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailService:Account"], ConfigurationManager.AppSettings["emailService:Password"]);
      var transportWeb = new Web(credentials);
      if (transportWeb != null) {
        await transportWeb.DeliverAsync(myMessage);
      } else {
        await Task.FromResult(0);
      }
    }
  }
}