

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


        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailjetClient client = new MailjetClient("bf041dabae3655ca0f85a57ceaec4d8b", "2b8652542f071430549132c763c4555d")
            {
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
                                {"Email", "3brakennus@gmail.com"},
                                {"Name", "3b"}
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