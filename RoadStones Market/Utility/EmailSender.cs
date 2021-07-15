

using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadStones_Market.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MailJetSettings MailJetSettings { get; set; }


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(MailJetSettings.ApiKey, MailJetSettings.SecretKey)
            { 
                //TODO I need to make sending of email work

               // Version = ApiVersion.V3_1,
               
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