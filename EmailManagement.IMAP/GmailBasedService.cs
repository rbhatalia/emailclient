using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using MailKit.Security;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace EmailClientCore
{
    public class GmailBasedService: IEmailAggregator
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;
        private readonly string _username;
        private readonly string _password;

        public GmailBasedService(string host, int port, bool useSsl, string username, string password)
        {
            _host = host;
            _port = port;
            _useSsl = useSsl;
            _username = username;
            _password = password;
        }

        public List<MimeMessage> FetchEmails(string emailId, DateTimeOffset lastRunDateTimeOffset)
        {
            using (var client = new ImapClient())
            {
                client.Connect(_host, _port, _useSsl);
                client.Authenticate(emailId, _password);

                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);

                var messages = new List<MimeMessage>();
                for (int i = 0; i < 10; i++)
                {
                    var message = inbox.GetMessage(i);
                    messages.Add(message);
                }

                client.Disconnect(true);
                return messages;
            }
        }
    }
}

