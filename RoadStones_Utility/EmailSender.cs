

using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace RoadStones_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public MailJetSettings _mailJetSettings { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

       


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey)
            { 
               //TODO Still something wrong and does not work properly --> need to be fixed
                //Version = ApiVersion.V3_1,

            };
            MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.Messages, new JArray {
                    new JObject {
                        {
                            "From",
                            new JObject {
                                {"Email", "middnight_man@protonmail.com"},
                                {"Name", "Proton"}
                            }
                        }, {
                            "To",
                            new JArray {
                                new JObject {
                                    {
                                        "Email",
                                        email
                                    }, {
                                        "Name",
                                        "3b"
                                    }
                                }
                            }
                        }, {
                            "Subject",
                            subject
                        },{
                            "HTMLPart",
                            body

                        }
                    }
                });
            await client.PostAsync(request);
        }
    }
}